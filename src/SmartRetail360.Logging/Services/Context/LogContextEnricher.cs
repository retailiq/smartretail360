using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Shared.Constants;
using SerilogContext = Serilog.Context.LogContext;

namespace SmartRetail360.Logging.Services.Context;

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
            SerilogContext.PushProperty("UserId", (userId ?? accessor.UserId)?.ToString() ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("TenantId", (tenantId ?? accessor.TenantId)?.ToString() ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("RoleId", (roleId ?? accessor.RoleId)?.ToString() ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("Module", accessor.Module ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("Locale", accessor.Locale ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("IpAddress", accessor.IpAddress ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("ErrorStack", accessor.ErrorStack ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("LogId", accessor.LogId ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("Action", accessor.Action ?? GeneralConstants.Unknown),
            SerilogContext.PushProperty("Env", accessor.Env ?? GeneralConstants.Unknown),
        };

        return new CompositeDisposable(disposables);
    }
}