using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Responses;
using System.Text.Json;
using SmartRetail360.Logging.Abstractions;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;

namespace SmartRetail360.Gateway.Middlewares;

public class JwtValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _jwtSecret;

    public JwtValidationMiddleware(
        RequestDelegate next,
        IConfiguration config
    )
    {
        _next = next;
        _jwtSecret = config.GetSection(GeneralConstants.App)[GeneralConstants.JwtSecret]!;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var log = context.RequestServices.GetRequiredService<ILogDispatcher>();
        var localizer = context.RequestServices.GetRequiredService<MessageLocalizer>();
        var userContext = context.RequestServices.GetRequiredService<IUserContextService>();

        userContext.Inject(new UserExecutionContext
        {
            Module = LogSourceModules.ValidateTokenMiddleware
        });

        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (TokenValidationRules.IsPathWhitelisted(path))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers[GeneralConstants.Authorization].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            await log.Dispatch(
                LogEventType.TokenValidationFailure,
                LogReasons.TokenMissing
            );
            await WriteUnauthorized(context, ErrorCodes.TokenMissing,
                localizer.GetErrorMessage(ErrorCodes.TokenMissing));
            return;
        }

        var token = authHeader["Bearer ".Length..];
        var handler = new JwtSecurityTokenHandler();
        ClaimsPrincipal? principal;
        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            principal = handler.ValidateToken(token, parameters, out var _);
        }
        catch (SecurityTokenExpiredException ex)
        {
            await log.Dispatch(
                LogEventType.TokenValidationFailure,
                LogReasons.TokenExpired
            );
            userContext.Inject(new UserExecutionContext
            {
                ErrorStack = ex.ToString()
            });
            await WriteUnauthorized(context, ErrorCodes.TokenExpired,
                localizer.GetErrorMessage(ErrorCodes.TokenExpired));
            return;
        }
        catch (Exception ex)
        {
            await log.Dispatch(
                LogEventType.TokenValidationFailure,
                LogReasons.TokenValidationFailed
            );
            userContext.Inject(new UserExecutionContext
            {
                ErrorStack = ex.ToString()
            });
            await WriteUnauthorized(context, ErrorCodes.TokenValidationFailed,
                localizer.GetErrorMessage(ErrorCodes.TokenValidationFailed));
            return;
        }

        var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);
        
        // Inject claims into headers
        void Inject(string claim, string header)
        {
            if (claims.TryGetValue(claim, out var value))
            {
                context.Request.Headers[header] = value;
            }
        }

        Inject("user_id", "X-User-Id");
        Inject("user_email", "X-Email");
        Inject("user_name", "X-User-Name");
        Inject("tenant_id", "X-Tenant-Id");
        Inject("role_id", "X-Role-Id");
        Inject("env", "X-Env");
        Inject("role_name", "X-Role-Name");

        if (claims.TryGetValue("tenant_id", out var tenantIdStr) && Guid.TryParse(tenantIdStr, out var tenantId))
        {
            var isSystemAccount = tenantId == GeneralConstants.SystemTenantId;
            context.Request.Headers["X-Is-System-Account"] = isSystemAccount.ToString().ToLower();
        }

        userContext.Inject(new UserExecutionContext
        {
            UserId = Guid.Parse(claims["user_id"]),
            TenantId = Guid.Parse(claims["tenant_id"]),
            RoleId = Guid.Parse(claims["role_id"]),
            Email = claims["user_email"],
            UserName = claims["user_name"],
            RoleName = claims["role_name"],
        });

        await _next(context);
    }

    private static Task WriteUnauthorized(HttpContext context, int code, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        var traceId = context.Items["TraceId"]?.ToString();
        var response = ApiResponse<object>.Fail(code, message, traceId);
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}