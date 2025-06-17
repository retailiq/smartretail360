using Microsoft.Extensions.Logging;
using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Logging.Services.Policies;

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
                WriteAudit = false,
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
                WriteAudit = false,
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
            (LogEventType.LoginFailure, LogReasons.LockNotAcquired),
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
            (LogEventType.LoginFailure, LogReasons.AccountLockedDueToLoginFailures),
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
            (LogEventType.LoginFailure, LogReasons.AccountNotActivated),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Security]
            }
        },
        {
            (LogEventType.LoginFailure, LogReasons.TokenNotFound),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Security]
            }
        },
        {
            (LogEventType.LoginFailure, LogReasons.AccountLocked),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Security]
            }
        },
        {
            (LogEventType.LoginFailure, LogReasons.AccountSuspended),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Security]
            }
        },
        {
            (LogEventType.LoginFailure, LogReasons.AccountDeleted),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Security]
            }
        },
        {
            (LogEventType.LoginFailure, LogReasons.AccountBanned),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Security]
            }
        },
        {
            (LogEventType.LoginFailure, LogReasons.AbacTenantUserRecordNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Information,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Security, LogCategory.Application]
            }
        },
        {
            (LogEventType.LoginFailure, LogReasons.TenantNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Application]
            }
        },
        {
            (LogEventType.LoginFailure, LogReasons.AllTenantsDisabled),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Application]
            }
        },
        {
            (LogEventType.LoginFailure, LogReasons.RoleListNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogAction = LogActions.UserLogin,
                LogCategories = [LogCategory.Application, LogCategory.Security]
            }
        },
        {
            (LogEventType.LoginSuccess, null),
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

        #region CredentialsLogin - Success & Failures

        {
            (LogEventType.CredentialsLoginFailure, LogReasons.AccountNotFound),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Information,
                LogAction = LogActions.CredentialsLogin,
                LogCategories = [LogCategory.Security]
            }
        },
        {
            (LogEventType.CredentialsLoginFailure, LogReasons.PasswordEmailMismatch),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.CredentialsLogin,
                LogCategories = [LogCategory.Security, LogCategory.Behavior]
            }
        },

        #endregion

        #region OAuth Login - Success & Failures

        {
            (LogEventType.OAuthLoginFailure, LogReasons.OAuthHandlerNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogAction = LogActions.OAuthLogin,
                LogCategories = [LogCategory.Security, LogCategory.Application]
            }
        },
        {
            (LogEventType.OAuthLoginFailure, LogReasons.OAuthUserProfileFetchFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogAction = LogActions.OAuthLogin,
                LogCategories = [LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.OAuthLoginFailure, LogReasons.OAuthUserProfileNotExists),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.OAuthLogin,
                LogCategories = [LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.OAuthLoginFailure, LogReasons.AbacTenantUserRecordNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.OAuthLogin,
                LogCategories = [LogCategory.Security, LogCategory.Application]
            }
        },
        {
            (LogEventType.CredentialsLoginFailure, LogReasons.RoleListNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogAction = LogActions.OAuthLogin,
                LogCategories = [LogCategory.Security, LogCategory.System, LogCategory.Application]
            }
        },

        #endregion

        #region ConfirmTenantLogin - Success & Failures

        {
            (LogEventType.ConfirmTenantLoginFailure, LogReasons.AbacTenantUserRecordNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.ConfirmTenantLogin,
                LogCategories = [LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.ConfirmTenantLoginFailure, LogReasons.TenantUserDisabled),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.ConfirmTenantLogin,
                LogCategories = [LogCategory.Security, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.ConfirmTenantLoginFailure, LogReasons.AccountNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Information,
                LogAction = LogActions.ConfirmTenantLogin,
                LogCategories = [LogCategory.Security]
            }
        },
        {
            (LogEventType.ConfirmTenantLoginFailure, LogReasons.TenantNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.ConfirmTenantLogin,
                LogCategories = [LogCategory.Application]
            }
        },
        {
            (LogEventType.ConfirmTenantLoginFailure, LogReasons.TenantDisabled),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogAction = LogActions.ConfirmTenantLogin,
                LogCategories = [LogCategory.Application]
            }
        },
        {
            (LogEventType.ConfirmTenantLoginFailure, LogReasons.RefreshTokenCreationFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogAction = LogActions.ConfirmTenantLogin,
                LogCategories = [LogCategory.Security, LogCategory.System]
            }
        },
        {
            (LogEventType.ConfirmTenantLoginSuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = true,
                LogLevel = LogLevel.Information,
                LogAction = LogActions.ConfirmTenantLogin,
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
                WriteAudit = false,
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
                WriteAudit = false,
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
                WriteAudit = false,
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
                WriteAudit = false,
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
            (LogEventType.EmailSendFailure, LogReasons.TokenNotFound),
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
                WriteAudit = false,
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
                WriteAudit = false,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogAction = LogActions.MatchEmailStrategy,
                LogCategories = [LogCategory.System, LogCategory.Application, LogCategory.Behavior]
            }
        },
        {
            (LogEventType.EmailSendFailure, LogReasons.HasPendingActivationEmail),
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
                WriteAudit = false,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System, LogCategory.Application],
                LogAction = LogActions.WriteAuditLog
            }
        },

        #endregion

        #region ABAC - Success & Failures

        {
            (LogEventType.AuthorizationFailure, LogReasons.AccountNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories =
                    [LogCategory.System, LogCategory.Application, LogCategory.Security, LogCategory.Behavior],
                LogAction = LogActions.AbacAccessAttempt
            }
        },
        {
            (LogEventType.AuthorizationFailure, LogReasons.AccessDeniedByPolicy),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories =
                    [LogCategory.System, LogCategory.Application, LogCategory.Security, LogCategory.Behavior],
                LogAction = LogActions.AbacAccessAttempt
            }
        },
        {
            (LogEventType.AbacPolicyEvaluationFailure, LogReasons.PolicyEvaluationFailed),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories =
                    [LogCategory.System, LogCategory.Application, LogCategory.Security, LogCategory.Behavior],
                LogAction = LogActions.EvaluateAbacPolicy
            }
        },
        {
            (LogEventType.AbacPolicyEvaluationFailure, LogReasons.PolicyNotFoundOrDisabled),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories =
                    [LogCategory.System, LogCategory.Application, LogCategory.Security, LogCategory.Behavior],
                LogAction = LogActions.EvaluateAbacPolicy
            }
        },
        {
            (LogEventType.GetAbacResolverFailure, LogReasons.AbacResolverNotFound),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories =
                    [LogCategory.System, LogCategory.Application, LogCategory.Security, LogCategory.Behavior],
                LogAction = LogActions.GetAbacResolver
            }
        },
        {
            (LogEventType.GetAbacResolverFailure, LogReasons.AbacResolverExecutionFailed),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories =
                    [LogCategory.System, LogCategory.Application, LogCategory.Security, LogCategory.Behavior],
                LogAction = LogActions.GetAbacResolver
            }
        },
        {
            (LogEventType.GetAbacResourceFailure, LogReasons.AbacTenantUserRecordNotFound),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.System, LogCategory.Application],
                LogAction = LogActions.GetAbacResource
            }
        },
        {
            (LogEventType.GetAbacResourceFailure, LogReasons.AbacUserTenantRoleNotFound),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.System, LogCategory.Application],
                LogAction = LogActions.GetAbacResource
            }
        },
        {
            (LogEventType.GetAbacResourceFailure, LogReasons.AbacResourceIdMissing),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Information,
                LogCategories = [LogCategory.Application, LogCategory.Behavior],
                LogAction = LogActions.GetAbacResource
            }
        },
        {
            (LogEventType.UpdateAbacPolicyFailure, LogReasons.InvalidAbacPolicyRule),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Information,
                LogCategories = [LogCategory.Application, LogCategory.Behavior, LogCategory.Security],
                LogAction = LogActions.UpdateAbacPolicy
            }
        },
        {
            (LogEventType.UpdateAbacPolicyFailure, LogReasons.AbacPolicyNotFound),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Application, LogCategory.Behavior, LogCategory.Security],
                LogAction = LogActions.UpdateAbacPolicy
            }
        },
        {
            (LogEventType.UpdateAbacPolicyFailure, LogReasons.AbacPolicyUnchanged),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Information,
                LogCategories = [LogCategory.Application, LogCategory.Behavior],
                LogAction = LogActions.UpdateAbacPolicy
            }
        },
        {
            (LogEventType.UpdateAbacPolicyFailure, LogReasons.AbacPolicyHasBeenReplaced),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Application, LogCategory.Security],
                LogAction = LogActions.UpdateAbacPolicy
            }
        },
        {
            (LogEventType.UpdateAbacPolicyFailure, LogReasons.AbacPolicyDisabled),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Application, LogCategory.Security],
                LogAction = LogActions.UpdateAbacPolicy
            }
        },
        {
            (LogEventType.UpdateAbacPolicySuccess, null),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = true,
                LogLevel = LogLevel.Information,
                LogCategories = [LogCategory.Application, LogCategory.Behavior, LogCategory.Security],
                LogAction = LogActions.UpdateAbacPolicy
            }
        },
        {
            (LogEventType.GetAbacPoliciesSuccess, null),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = false,
                IsSuccess = true,
                LogLevel = LogLevel.Information,
                LogCategories = [LogCategory.Application, LogCategory.Behavior],
                LogAction = LogActions.GetAbacPolicies
            }
        },

        #endregion

        #region General

        {
            (LogEventType.DatabaseError, LogReasons.DatabaseSaveFailed),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System]
            }
        },
        {
            (LogEventType.DatabaseError, LogReasons.DatabaseRetrievalFailed),
            new LogWriteRule
            {
                WriteAudit = false,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System]
            }
        },
        {
            (LogEventType.RedisError, LogReasons.RedisOperateFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.System]
            }
        },

        #endregion

        #region Auth

        {
            (LogEventType.RefreshTokenReplayDetected, LogReasons.RefreshTokenReplayDetected),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = true,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.System, LogCategory.Security]
            }
        },
        {
            (LogEventType.TokenValidationFailure, LogReasons.TokenMissing),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.System],
                LogAction = LogActions.ValidateAccessToken
            }
        },
        {
            (LogEventType.TokenValidationFailure, LogReasons.TokenExpired),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.System, LogCategory.Behavior],
                LogAction = LogActions.ValidateAccessToken
            }
        },
        {
            (LogEventType.TokenValidationFailure, LogReasons.TokenValidationFailed),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.System, LogCategory.Behavior],
                LogAction = LogActions.ValidateAccessToken
            }
        },
        {
            (LogEventType.TokenValidationFailure, LogReasons.InvalidToken),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Error,
                LogCategories = [LogCategory.Security, LogCategory.System],
                LogAction = LogActions.ValidateAccessToken
            }
        },
        {
            (LogEventType.RefreshTokenFailure, LogReasons.RefreshTokenMissing),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.System]
            }
        },
        {
            (LogEventType.RefreshTokenFailure, LogReasons.RefreshTokenExpired),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.System]
            }
        },
        {
            (LogEventType.RefreshTokenFailure, LogReasons.RefreshTokenRevoked),
            new LogWriteRule
            {
                WriteAudit = true,
                SendToSentry = false,
                IsSuccess = false,
                LogLevel = LogLevel.Warning,
                LogCategories = [LogCategory.Security, LogCategory.System]
            }
        }

        #endregion
    };

    public LogWriteRule GetPolicy(LogEventType eventType, string? reason = null)
    {
        return Rules.TryGetValue((eventType, reason), out var rule)
            ? rule
            : new LogWriteRule
            {
                LogCategories = [],
                LogLevel = LogLevel.None,
                WriteAudit = false,
                SendToSentry = false,
                IsSuccess = false,
                LogAction = null
            };
    }
}