using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging.Policies;

public class DefaultLogWritePolicyProvider : ILogWritePolicyProvider
{
    private static readonly Dictionary<(LogEventType, string?), LogWriteRule> Rules = new()
    {
        #region Register - Success & Failures

        {
            (LogEventType.RegisterFailure, LogReasons.SendSqsMessageFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.RegisterFailure, LogReasons.LockNotAcquired),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.RegisterFailure, LogReasons.DatabaseSaveFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.RegisterFailure, LogReasons.DatabaseRetrievalFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
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

        #endregion

        #region Account Activate - Success & Failures

        {
            (LogEventType.AccountActivateFailure, LogReasons.InvalidTokenOrAccountAlreadyActivated),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategory = LogCategory.Security
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.DatabaseSaveFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.DatabaseRetrievalFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.TooFrequentActivationAttempt),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.AccountActivateSuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = true,
                LogLevel = LogLevel.Information,
                LogCategory = LogCategory.Application
            }
        },

        #endregion

        #region Email Sending - Success & Failures

        {
            (LogEventType.EmailSendFailure, LogReasons.TooFrequentEmailRequest),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategory = LogCategory.Application
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.SendSqsMessageFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.EmailSendFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.DatabaseSaveFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.DatabaseRetrievalFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategory = LogCategory.System
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.TenantAccountAlreadyActivated),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategory = LogCategory.Application
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.TenantNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategory = LogCategory.Application
            }
        },
        {
            (LogEventType.EmailSendSuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = false,
                IsSuccess = true,
                LogLevel = LogLevel.Information,
                LogCategory = LogCategory.Application
            }
        },

        #endregion

        #region Logs - Failures
        
        {
            (LogEventType.WriteLogFailure, LogReasons.DatabaseSaveFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                WriteSystemLog = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategory = LogCategory.System,
                LogAction = LogActions.WriteAuditLog
            }
        }

        #endregion
    };

    public LogWriteRule GetPolicy(LogEventType eventType, string? reason = null)
    {
        return Rules.TryGetValue((eventType, reason), out var rule)
            ? rule
            : new LogWriteRule { WriteAudit = true };
    }
}
// Default Mode: Audit 