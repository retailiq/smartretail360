using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Infrastructure.Logging.Context;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging.Context;

public static class LogContextPushHelper
{
    public static IDisposable Push(LogContext context, ILogContextAccessor accessor)
    {
        return LogContextEnricher.EnrichFromContext(
            accessor,
            traceId: context.TraceId,
            email: context.Email,
            userId: context.UserId,
            tenantId: context.TenantId,
            roleId: context.RoleId
        );
    }
}