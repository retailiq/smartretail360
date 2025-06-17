using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Messaging.Payloads;

namespace SmartRetail360.Shared.Contexts.User;

public static class UserContextExtensions
{
    public static void ApplyTo(this IUserContextService context, ActivationEmailPayload payload)
    {
        payload.TenantId = context.TenantId ?? Guid.Empty;
        payload.TraceId = context.TraceId;
        payload.Module = context.Module;
        payload.Locale = context.Locale ?? "en";
        payload.IpAddress = context.IpAddress;
        payload.UserId = context.UserId ?? Guid.Empty;
        payload.RoleId = context.RoleId ?? Guid.Empty;
        payload.RoleName = context.RoleName ?? GeneralConstants.Unknown;
        payload.LogId = context.LogId ?? GeneralConstants.Unknown;
    }
}