using SmartRetail360.Application.Interfaces.Logging;
using SerilogContext = Serilog.Context.LogContext;

namespace SmartRetail360.Infrastructure.Logging.Context;

public static class LogContextEnricher
{
    public static IDisposable EnrichFromContext(ILogContextAccessor accessor)
    {
        var disposables = new List<IDisposable>
        {
            SerilogContext.PushProperty("TraceId", accessor.TraceId ?? "unknown"),
            SerilogContext.PushProperty("ClientEmail", accessor.ClientEmail ?? "unknown"),
            SerilogContext.PushProperty("Locale", accessor.Locale ?? "unknown"),
            SerilogContext.PushProperty("UserId", accessor.UserId?.ToString() ?? "unknown"),
            SerilogContext.PushProperty("TenantId", accessor.TenantId?.ToString() ?? "unknown"),
            SerilogContext.PushProperty("RoleId", accessor.RoleId?.ToString() ?? "unknown"),
            SerilogContext.PushProperty("Module", accessor.Module ?? "unknown"),
            SerilogContext.PushProperty("IpAddress", accessor.IpAddress ?? "unknown"),
            SerilogContext.PushProperty("AccountType", accessor.AccountType ?? "unknown")
        };

        return new CompositeDisposable(disposables);
    }
}