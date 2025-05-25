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
            (LogEventType.RegisterUserFailure, LogReasons.SendSqsMessageFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System]
            }
        },
        {
            (LogEventType.RegisterUserFailure, LogReasons.LockNotAcquired),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.System, LogCategory.Application]
            }
        },
        {
            (LogEventType.RegisterUserFailure, LogReasons.DatabaseSaveFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Application]
            }
        },
        {
            (LogEventType.RegisterUserFailure, LogReasons.DatabaseRetrievalFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Application]
            }
        },
        {
            (LogEventType.RegisterUserFailure, LogReasons.AccountAlreadyExists),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.RegisterUserFailure, LogReasons.AccountExistsButNotActivated),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.RegisterUserSuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = true,
                LogLevel = LogLevel.Information,
                LogCategories = [LogCategory.Application, LogCategory.Behavior]
            }
        },

        #endregion

        #region Login - Success & Failures

        {
            (LogEventType.UserLoginFailure, LogReasons.PasswordEmailMismatch),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.UserLoginSuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                LogLevel = LogLevel.Information,
                LogAction = LogActions.UserLogin,
                IsSuccess = true,
                LogCategories = [LogCategory.Security, LogCategory.Application, LogCategory.Behavior]
            }
        },

        #endregion

        #region Account Activate - Success & Failures

        {
            (LogEventType.AccountActivateFailure, LogReasons.InvalidToken),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.TokenAlreadyUsed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.TokenExpired),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.TokenRevoked),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.HasPendingActivationEmail),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.DatabaseSaveFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.DatabaseRetrievalFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.AccountAlreadyActivated),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.TooFrequentActivationAttempt),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.System, LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.TokenDeserializationFailed),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateFailure, LogReasons.RoleDeserializationFailed),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.AccountActivateSuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = true,
                LogLevel = LogLevel.Information,
                LogCategories = [LogCategory.Application, LogCategory.Behavior]
            }
        },

        #endregion

        #region Email Sending - Success & Failures

        {
            (LogEventType.EmailSendFailure, LogReasons.TooFrequentEmailRequest),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Application, LogCategory.System, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.SendSqsMessageFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.EmailSendFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.DatabaseSaveFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.DatabaseRetrievalFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.AccountAlreadyActivated),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.AccountNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.EmailTemplateNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogAction = LogActions.MatchEmailTemplate,
                LogCategories = [LogCategory.System, LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.EmailStrategyNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogAction = LogActions.MatchEmailStrategy,
                LogCategories = [LogCategory.System, LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.EmailSendSuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = true,
                LogLevel = LogLevel.Information,
                LogCategories = [LogCategory.Application, LogCategory.Behavior]
            }
        },

        #endregion

        #region Logs - Failures

        {
            (LogEventType.WriteLogFailure, LogReasons.DatabaseSaveFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Application],
                LogAction = LogActions.WriteAuditLog
            }
        }

        #endregion
    };

    public LogWriteRule GetPolicy(LogEventType eventType, string? reason = null)
    {
        return Rules.TryGetValue((eventType, reason), out var rule)
            ? rule
            : new LogWriteRule { LogCategories = [LogCategory.Application] };
    }
}