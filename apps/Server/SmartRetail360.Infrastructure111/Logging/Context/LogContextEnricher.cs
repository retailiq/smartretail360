using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Shared.Constants;
using SerilogContext = Serilog.Context.LogContext;

namespace SmartRetail360.Infrastructure.Logging.Context;

public static class LogContextEnricher
{
    public static IDisposable EnrichFromContext(
        ILogContextAccessor accessor,
        string? traceId = null,
        string? email = null,
        Guid? userId = null,
        Guid? tenantId = null,
        Guid? roleId = null)
    {
        var disposables = new List<IDisposable>
        {
            SerilogContext.PushProperty("TraceId", traceId ?? accessor.TraceId ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("ClientEmail", email ?? accessor.ClientEmail ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("UserId", (userId ?? accessor.UserId)?.ToString() ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("TenantId", (tenantId ?? accessor.TenantId)?.ToString() ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("RoleId", (roleId ?? accessor.RoleId)?.ToString() ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("Module", accessor.Module ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("Locale", accessor.Locale ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("IpAddress", accessor.IpAddress ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("AccountType", accessor.AccountType ?? GeneralConstants.Unknown)
        };

        return new CompositeDisposable(disposables);
    }
}