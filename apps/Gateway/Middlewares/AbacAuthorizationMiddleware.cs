using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using SmartRetail360.ABAC.Services;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Execution;
using SmartRetail360.Logging.Abstractions;
using SmartRetail360.Persistence.Data;

namespace SmartRetail360.Gateway.Middlewares;

public class AbacAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AbacAuthorizationMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Normalize path (remove prefix like "/server" for matching)
        var rawPath = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        var path = rawPath;

        // Step 1: Match the path to an ABAC route rule
        var mapper = context.RequestServices.GetRequiredService<AbacRouteMapper>();
        var (resourceType, action) = mapper.Map(path);
        if (resourceType == GeneralConstants.Unknown || action == GeneralConstants.Unknown)
        {
            await _next(context);
            return;
        }

        // Step 2: Resolve required services from DI
        var userContext = context.RequestServices.GetRequiredService<IUserContextService>();
        var evaluator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();
        var resolver = context.RequestServices.GetRequiredService<IResourceAttributeResolver>();
        var localizer = context.RequestServices.GetRequiredService<MessageLocalizer>();
        var log = context.RequestServices.GetRequiredService<ILogDispatcher>();
        var execution = context.RequestServices.GetRequiredService<ISafeExecutor>();
        var db = context.RequestServices.GetRequiredService<AppDbContext>();

        userContext.Inject(new UserExecutionContext
        {
            Module = LogSourceModules.AbacAuthorizationMiddleware,
        });

        // Step 3: Extract resource ID from the path
        var resourceId = ExtractResourceId(path);
        var tenantId = userContext.TenantId ?? Guid.Empty;
        var userId = userContext.UserId ?? Guid.Empty;
        var roleId = userContext.RoleId ?? Guid.Empty;
        var roleName = userContext.RoleName ?? string.Empty;
        var email = userContext.Email ?? string.Empty;
        var userName = userContext.UserName ?? string.Empty;
        var env = context.Request.Headers["X-Env"].ToString();
        var traceId = context.Request.Headers["X-Trace-Id"].ToString();

        var userResult = await execution.ExecuteAsync(
            () => db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.None
        );
        if (!userResult.IsSuccess)
        {
            await WriteJsonErrorResponseAsync(context, StatusCodes.Status500InternalServerError,
                ErrorCodes.DatabaseUnavailable, traceId, localizer);
            return;
        }

        var user = userResult.Response.Data;

        if (user == null)
        {
            await log.Dispatch(LogEventType.AuthorizationFailure, LogReasons.AccountNotFound);
            await WriteJsonErrorResponseAsync(context, StatusCodes.Status404NotFound,
                ErrorCodes.AccountNotFound, traceId, localizer);
            return;
        }

        var resourceAttributes = await resolver.ResolveAsync(resourceType, resourceId);
        var isSystemAccount = tenantId == (Guid?)GeneralConstants.SystemTenantId;
        var accessAllowed = user.StatusEnum == AccountStatus.Active;

        var evalContext = new
        {
            user = new
            {
                id = userId,
                tenant_id = tenantId,
                role_id = roleId,
                email,
                name = userName,
                role_name = roleName,
                is_system_account = isSystemAccount,
            },
            resource = resourceAttributes,
            environment = new { name = env },
            tenant_constraints = new { access_allowed = accessAllowed },
        };
        
        Console.WriteLine("resourceType: " + resourceType);
        Console.WriteLine("action: " + action);
        
        // Step 5: Evaluate policy
        var allowed = await evaluator.EvaluateAsync(tenantId, resourceType, action, evalContext);
        if (!allowed)
        {
            await WriteJsonErrorResponseAsync(context, StatusCodes.Status403Forbidden,
                ErrorCodes.AuthorizationFailure, traceId, localizer);
            await log.Dispatch(LogEventType.AuthorizationFailure, LogReasons.AccessDeniedByPolicy);
            return;
        }

        // Step 6: Proceed to next middleware if authorized
        await _next(context);
    }

    // Extracts a GUID resource ID from the path (e.g. last segment)
    private static string? ExtractResourceId(string path)
    {
        var segments = path.Split('/');
        var id = segments.LastOrDefault(s => Guid.TryParse(s, out _));
        return id;
    }

    private static async Task WriteJsonErrorResponseAsync(
        HttpContext context,
        int statusCode,
        int errorCode,
        string traceId,
        MessageLocalizer localizer)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var response = ApiResponse<object>.Fail(
                errorCode,
                localizer.GetErrorMessage(errorCode),
                traceId
            );
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}