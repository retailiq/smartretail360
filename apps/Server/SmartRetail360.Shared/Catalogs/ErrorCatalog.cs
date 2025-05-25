using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Shared.Catalogs;

public static class ErrorCatalog
{
    private static readonly Dictionary<int, string> Keys = new()
    {
        // General errors
        { ErrorCodes.InternalServerError, "InternalServerError" },
        { ErrorCodes.DatabaseUnavailable, "DatabaseUnavailable" },
        { ErrorCodes.DuplicateRegisterAttempt, "DuplicateRegisterAttempt" },

        // Auth
        { ErrorCodes.InvalidPassword, "InvalidPassword" },
        { ErrorCodes.InvalidToken, "InvalidToken" },
        { ErrorCodes.TokenExpired, "TokenExpired" },
        { ErrorCodes.TokenAlreadyUsed, "TokenAlreadyUsed" },
        { ErrorCodes.TokenRevoked, "TokenRevoked" },
        { ErrorCodes.HasPendingActivationEmail, "HasPendingActivationEmail" },
        { ErrorCodes.PasswordEmailMismatch, "PasswordEmailMismatch" },
        { ErrorCodes.AccountNotActivated, "AccountNotActivated" },

        // Account
        { ErrorCodes.AccountLocked, "AccountLocked" },
        { ErrorCodes.AccountNotFound, "AccountNotFound" },
        { ErrorCodes.AccountAlreadyActivated, "AccountAlreadyActivated" },
        { ErrorCodes.AccountExistsButNotActivated, "AccountExistsButNotActivated" },
        { ErrorCodes.TooFrequentActivationAttempt, "TooFrequentActivationAttempt" },

        // Email & Notification
        { ErrorCodes.EmailExists, "EmailExists" },
        { ErrorCodes.TooFrequentEmailRequest, "TooFrequentEmailRequest" },
        { ErrorCodes.EmailSendFailed, "EmailSendFailed" },
        { ErrorCodes.EmailTemplateNotFound, "UnsupportedEmailTemplate" },
        { ErrorCodes.EmailStrategyNotFound, "EmailStrategyNotFound" },
    };

    public static string GetKey(int code)
    {
        return Keys.GetValueOrDefault(code, GeneralConstants.Unknown);
    }
}