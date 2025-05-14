using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging;

public class DefaultLogWriter : ILogWriter
{
    private readonly IAuditLogger _auditLogger;
    private readonly IUserContextService _userContext;

    public DefaultLogWriter(IAuditLogger auditLogger, IUserContextService userContext)
    {
        _auditLogger = auditLogger;
        _userContext = userContext;
    }

    public async Task WriteAsync(LogContext context, LogWriteRule rule)
    {
        var tasks = new List<Task>();

        if (rule.WriteAudit)
        {
            tasks.Add(_auditLogger.LogAsync(new AuditContext
            {
                Action = context.Action ?? "UNKNOWN_ACTION",
                IsSuccess = context.IsSuccess,
                Email = context.Email ?? _userContext.ClientEmail ?? "unknown",
                Reason = context.Reason,
                ErrorStack = context.ErrorStack,
                Level = rule.SystemLogLevel,
                SourceModule = _userContext.Module ?? LogSourceModules.Unknown
            }));
        }

        if (rule.WriteSystemLog)
        {
            tasks.Add(Task.Run(() =>
            {
                Console.WriteLine($">>> [LOG] [{rule.SystemLogCategory}] {rule.SystemLogAction} | Level: {rule.SystemLogLevel} | TraceId: {_userContext.TraceId}");
            }));
        }

        await Task.WhenAll(tasks);
    }
}