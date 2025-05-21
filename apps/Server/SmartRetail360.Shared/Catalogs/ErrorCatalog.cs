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

        // Account
        { ErrorCodes.TenantNotFound, "TenantNotFound" },
        { ErrorCodes.AccountLocked, "AccountLocked" },
        { ErrorCodes.AccountNotFound, "AccountNotFound" },
        { ErrorCodes.AccountAlreadyActivated, "AccountAlreadyActivated" },
        { ErrorCodes.AccountExistsButNotActivated, "AccountExistsButNotActivated" },
        { ErrorCodes.InvalidTokenOrAccountAlreadyActivated, "InvalidTokenOrAccountAlreadyActivated" },
        { ErrorCodes.TooFrequentActivationAttempt, "TooFrequentActivationAttempt" },
        
        // Email & Notification
        { ErrorCodes.EmailExists, "EmailExists" },
        { ErrorCodes.TooFrequentEmailRequest, "TooFrequentEmailRequest" },
        { ErrorCodes.EmailSendFailed, "EmailSendFailed" },
        { ErrorCodes.EmailTemplateNotFound, "UnsupportedEmailTemplate"},
        { ErrorCodes.EmailStrategyNotFound, "EmailStrategyNotFound" },
    };

    public static string GetKey(int code)
    {
        return Keys.GetValueOrDefault(code, GeneralConstants.Unknown);
    }
}