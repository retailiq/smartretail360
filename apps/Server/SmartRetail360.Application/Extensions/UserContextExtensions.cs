using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Shared.DTOs.Messaging;

namespace SmartRetail360.Application.Extensions;

public static class UserContextExtensions
{
    public static void ApplyTo(this IUserContextService context, ActivationEmailPayload payload)
    {
        payload.TenantId = context.TenantId ?? Guid.Empty;
        payload.TraceId = context.TraceId ?? Guid.NewGuid().ToString("N");
        payload.Module = context.Module;
        payload.Locale = context.Locale ?? "en";
        payload.AccountType = context.AccountType;
        payload.IpAddress = context.IpAddress;
        payload.Action = context.Action;
    }
}