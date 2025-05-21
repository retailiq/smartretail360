using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Infrastructure.Logging.Context;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging;

public class DefaultLogWriter : ILogWriter
{
    private readonly IAuditLogger _auditLogger;
    private readonly ILogContextAccessor _logContextAccessor;

    public DefaultLogWriter(
        IAuditLogger auditLogger,
        ILogContextAccessor logContextAccessor)
    {
        _auditLogger = auditLogger;
        _logContextAccessor = logContextAccessor;
    }

    public async Task WriteAsync(LogContext context, LogWriteRule rule)
    {
        var tasks = new List<Task>();

        if (rule.WriteAudit)
        {
            tasks.Add(_auditLogger.LogAsync(new AuditContext
            {
                LogId = context.LogId,
                Reason = context.Reason,
                Action = rule.LogAction ?? _logContextAccessor.Action ?? GeneralConstants.Unknown,
                IsSuccess = rule.IsSuccess ?? true,
                Email = _logContextAccessor.ClientEmail ?? GeneralConstants.Unknown,
                ErrorStack = _logContextAccessor.ErrorStack,
                Level = rule.LogLevel,
                SourceModule = _logContextAccessor.Module ?? GeneralConstants.Unknown,
                UserId = _logContextAccessor.UserId,
                TenantId = _logContextAccessor.TenantId,
                Category = rule.LogCategory ?? LogCategory.Application
            }));
        }

        if (rule.WriteSystemLog)
        {
            tasks.Add(Task.Run(() =>
            {
                using var _ = LogContextEnricher.EnrichFromContext(_logContextAccessor);

                using (LogContextEnricher.EnrichFromContext(_logContextAccessor))
                {
                    Log.Write(rule.LogLevel switch
                        {
                            LogLevel.Error => LogEventLevel.Error,
                            LogLevel.Warning => LogEventLevel.Warning,
                            _ => LogEventLevel.Information
                        },
                        "[{Category}] {Action} | Email: {Email} | Success: {IsSuccess} | Reason: {Reason} | LogId: {LogId}",
                        rule.LogCategory,
                        rule.LogAction ?? _logContextAccessor.Action ?? GeneralConstants.Unknown,
                        _logContextAccessor.ClientEmail ?? GeneralConstants.Unknown,
                        rule.IsSuccess ?? true,
                        context.Reason ?? "-",
                        context.LogId ?? Guid.NewGuid().ToString()
                    );
                }
            }));
        }

        if (rule.SendToSentry)
        {
            if (!string.IsNullOrWhiteSpace(_logContextAccessor.ErrorStack))
            {
                tasks.Add(Task.Run(() =>
                {
                    var ex = new Exception(_logContextAccessor.ErrorStack);
                    SentrySdk.CaptureException(ex);
                }));
            }
            else
            {
                tasks.Add(Task.Run(() =>
                {
                    SentrySdk.CaptureMessage(
                        $"[{rule.LogCategory}] {rule.LogAction}: {context.Reason ?? "N/A"}",
                        SentryLevel.Warning
                    );
                }));
            }
        }

        await Task.WhenAll(tasks);
    }
}