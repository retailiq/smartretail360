using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Auth.AccessControl;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Application.Common.AccessControl;

public class AbacAuthorizationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get attributes from the controller or action
        var descriptor = context.ActionDescriptor;
        var attributes = descriptor.EndpointMetadata.OfType<AccessControlAttribute>().ToList();

        if (!attributes.Any())
        {
            await next();
            return;
        }

        var userContext = context.HttpContext.RequestServices.GetRequiredService<IUserContextService>();
        var policyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
        var resourceResolver = context.HttpContext.RequestServices.GetRequiredService<IResourceAttributeResolver>();
        var localizer = context.HttpContext.RequestServices.GetRequiredService<MessageLocalizer>();
        var logDispatcher = context.HttpContext.RequestServices.GetRequiredService<ILogDispatcher>();

        foreach (var attr in attributes)
        {
            var resourceId = context.ActionArguments.TryGetValue("id", out var val) ? val?.ToString() : null;
            var tenantId = userContext.TenantId ?? Guid.NewGuid();
            var resourceAttributes = await resourceResolver.ResolveAsync(attr.ResourceType, resourceId);
            
            var env = new
            {
                name = userContext.Env.GetEnumMemberValue(),
            };
            
            var evaluationContext = new
            {
                user = new
                {
                    id = userContext.UserId,
                    tenant_id = userContext.TenantId,
                    role_id = userContext.RoleId,
                    email = userContext.Email,
                    name = userContext.UserName
                },
                resource = resourceAttributes,
                environment = env,
                tenant_constraints = new
                {
                    // Get from DB or redis later
                    access_allowed = true
                }
            };
            
            var allowed =
                await policyEvaluator.EvaluateAsync(tenantId, attr.ResourceType, attr.Action, evaluationContext);

            if (!allowed)
            {
                context.Result = new ObjectResult(
                    ApiResponse<object>.Fail(
                        ErrorCodes.AuthorizationFailure,
                        localizer.GetErrorMessage(ErrorCodes.AuthorizationFailure),
                        userContext.TraceId
                    )
                )
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };

                userContext.Inject(new UserExecutionContext
                {
                    Action = LogActions.AbacAccessAttempt
                });

                await logDispatcher.Dispatch(LogEventType.AuthorizationFailure, LogReasons.AccessDeniedByPolicy);

                return;
            }
        }

        await next();
    }
}