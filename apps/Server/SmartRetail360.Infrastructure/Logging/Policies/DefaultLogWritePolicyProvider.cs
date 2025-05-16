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
        #region Register - Success & Failures
        
        {
            (LogEventType.RegisterFailure, LogReasons.EmailSendFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogAction = LogActions.TenantRegister,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.RegisterFailure, LogReasons.LockNotAcquired),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.TenantRegister,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.RegisterFailure, LogReasons.DatabaseOperationFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.TenantRegister,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.RegisterFailure, LogReasons.TenantAccountAlreadyExists),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.TenantRegister,
                LogCategory = LogCategory.Application
            }
        },
        {
            (LogEventType.RegisterFailure, LogReasons.TenantAccountExistsButNotActivated),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.TenantRegister,
                LogCategory = LogCategory.Application
            }
        },
        {
            (LogEventType.RegisterSuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = true,
                LogAction = LogActions.TenantRegister,
                LogLevel = LogLevel.Information,
                LogCategory = LogCategory.Application
            }
        },
        
        #endregion
        
        #region Login - Success & Failures
        
        {
            (LogEventType.LoginFailure, LogReasons.InvalidCredentials),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                LogCategory = LogCategory.Security
            }
        },
        {
            (LogEventType.LoginSuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                IsSuccess = true,
                LogCategory = LogCategory.Security
            }
        },
        {
            (LogEventType.CopilotQuery, null),
            new LogWriteRule
            {
                WriteAudit = false,
                WriteSystemLog = true,
                LogLevel = LogLevel.Information
            }
        }
        
        #endregion
    };

    public LogWriteRule GetPolicy(LogEventType eventType, string? reason = null)
    {
        return _rules.TryGetValue((eventType, reason), out var rule)
            ? rule
            : new LogWriteRule { WriteAudit = true };
    }
}
// Default Mode: Audit 