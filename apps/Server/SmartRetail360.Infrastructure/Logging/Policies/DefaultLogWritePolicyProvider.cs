using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging.Policies;

public class DefaultLogWritePolicyProvider : ILogWritePolicyProvider
{
    private static readonly Dictionary<(LogEventType, string?), LogWriteRule> _rules = new()
    {
        {
            (LogEventType.RegisterFailure, LogReasons.EmailSendFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SystemLogAction = LogActions.SendEmailFailed,
                SystemLogCategory = LogCategory.System,
                SystemLogLevel = LogLevel.Error
            }
        },
        {
            (LogEventType.RegisterFailure, LogReasons.LockNotAcquired),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SystemLogAction = LogActions.GenerateAccountLockFailed,
                SystemLogCategory = LogCategory.System,
                SystemLogLevel = LogLevel.Error
            }
        },
        {
            (LogEventType.RegisterFailure, LogReasons.TenantAlreadyExists),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SystemLogLevel = LogLevel.Error
            }
        },
        {
            (LogEventType.RegisterSuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SystemLogAction = "RegisterSuccess",
                SystemLogCategory = LogCategory.Application,
                SystemLogLevel = LogLevel.Information
            }
        }
    };

    public LogWriteRule GetPolicy(LogEventType eventType, string? reason = null)
    {
        return _rules.TryGetValue((eventType, reason), out var rule)
            ? rule
            : new LogWriteRule { WriteAudit = true }; // 默认策略：只写审计日志
    }
}