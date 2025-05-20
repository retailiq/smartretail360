using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Shared.Catalogs;

public static class ErrorCatalog
{
    private static readonly Dictionary<int, string> _keys = new()
    {
        // General errors
        { ErrorCodes.InternalServerError, "InternalServerError" },
        { ErrorCodes.DatabaseUnavailable, "DatabaseUnavailable" },
        { ErrorCodes.UnsupportedEmailTemplate, "UnsupportedEmailTemplate"},
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
        
        // Email-related
        { ErrorCodes.EmailExists, "EmailExists" },
        { ErrorCodes.TooFrequentEmailRequest, "TooFrequentEmailRequest" },
        { ErrorCodes.EmailSendFailed, "EmailSendFailed" }
    };

    public static string GetKey(int code)
    {
        return _keys.TryGetValue(code, out var key) ? key : "UnknownError";
    }
}