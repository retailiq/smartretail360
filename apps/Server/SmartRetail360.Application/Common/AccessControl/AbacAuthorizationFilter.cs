using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Auth.AccessControl;
using SmartRetail360.Application.Interfaces.Auth.Resource;

namespace SmartRetail360.Application.Common.AccessControl;

public class AbacAuthorizationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
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

        foreach (var attr in attributes)
        {
            var resourceId = context.ActionArguments.TryGetValue("id", out var val) ? val?.ToString() : null;
            var resourceAttributes = await resourceResolver.ResolveAsync(attr.ResourceType, resourceId);

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
                resource = resourceAttributes
            };

            var allowed = await policyEvaluator.EvaluateAsync(attr.ResourceType, attr.Action, evaluationContext);

            if (!allowed)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        await next();
    }
}