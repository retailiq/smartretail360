using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Infrastructure.Logging.Context;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging;

public class DefaultLogWriter : ILogWriter
{
    private readonly IAuditLogger _auditLogger;
    private readonly IUserContextService _userContext;
    private readonly ILogContextAccessor _logContextAccessor;

    public DefaultLogWriter(
        IAuditLogger auditLogger,
        IUserContextService userContext,
        ILogContextAccessor logContextAccessor)
    {
        _auditLogger = auditLogger;
        _userContext = userContext;
        _logContextAccessor = logContextAccessor;
    }

    public async Task WriteAsync(LogContext context, LogWriteRule rule)
    {
        context.Action ??= rule.LogAction;
        context.IsSuccess = rule.IsSuccess ?? context.IsSuccess;
        context.LogCategory ??= rule.LogCategory;

        var tasks = new List<Task>();

        if (rule.WriteAudit)
        {
            tasks.Add(_auditLogger.LogAsync(new AuditContext
            {
                LogId = context.LogId,
                Action = context.Action ?? LogActions.UnknownError,
                IsSuccess = context.IsSuccess,
                Email = context.Email ?? _userContext.ClientEmail ?? GeneralConstants.Unknown,
                Reason = context.Reason,
                ErrorStack = context.ErrorStack,
                Level = rule.LogLevel,
                SourceModule = _userContext.Module ?? LogSourceModules.Unknown,
                UserId = context.UserId,
                TenantId = context.TenantId
            }));
        }

        if (rule.WriteSystemLog)
        {
            Console.WriteLine("Writing system log...");
            
            tasks.Add(Task.Run(() =>
            {
                using var _ = LogContextEnricher.EnrichFromContext(_logContextAccessor);

                using (LogContextPushHelper.Push(context, _logContextAccessor))
                {
                    Log.Write(rule.LogLevel switch
                        {
                            LogLevel.Error => LogEventLevel.Error,
                            LogLevel.Warning => LogEventLevel.Warning,
                            _ => LogEventLevel.Information
                        },
                        "[{Category}] {Action} | Email: {Email} | Success: {IsSuccess} | Reason: {Reason}",
                        rule.LogCategory,
                        rule.LogAction ?? context.Action,
                        context.Email ?? _userContext.ClientEmail ?? GeneralConstants.Unknown,
                        context.IsSuccess,
                        context.Reason ?? "-"
                    );
                }
            }));
        }

        if (rule.SendToSentry)
        {
            if (!string.IsNullOrWhiteSpace(context.ErrorStack))
            {
                tasks.Add(Task.Run(() =>
                {
                    var ex = new Exception(context.ErrorStack);
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