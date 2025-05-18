using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Shared.Catalogs;

public static class ErrorCatalog
{
    private static readonly Dictionary<int, string> _keys = new()
    {
        // General errors
        { ErrorCodes.InternalServerError, "InternalServerError" },
        { ErrorCodes.DatabaseUnavailable, "DatabaseUnavailable" },
        { ErrorCodes.ValidationFailed, "ValidationFailed" },
        { ErrorCodes.UnsupportedEmailTemplate, "UnsupportedEmailTemplate"},
        { ErrorCodes.DuplicateRegisterAttempt, "DuplicateRegisterAttempt" },

        // Authorization / Access control
        { ErrorCodes.Unauthorized, "Unauthorized" },
        { ErrorCodes.Forbidden, "Forbidden" },

        // Tenant / Account-related errors
        { ErrorCodes.EmailExists, "EmailExists" },
        { ErrorCodes.TenantNotFound, "TenantNotFound" },
        { ErrorCodes.InvalidPassword, "InvalidPassword" },
        { ErrorCodes.AccountLocked, "AccountLocked" },
        { ErrorCodes.AccountNotFound, "AccountNotFound" },
        { ErrorCodes.InvalidToken, "InvalidToken" },
        { ErrorCodes.TokenExpired, "TokenExpired" },
        { ErrorCodes.AccountAlreadyActivated, "AccountAlreadyActivated" },
        { ErrorCodes.AccountExistsButNotActivated, "AccountExistsButNotActivated" },

        // Operation restrictions
        { ErrorCodes.RateLimitExceeded, "RateLimitExceeded" },
        { ErrorCodes.OperationNotAllowed, "OperationNotAllowed" },
        { ErrorCodes.TooFrequentEmailRequest, "TooFrequentEmailRequest" },
        { ErrorCodes.InvalidTokenOrAccountAlreadyActivated, "InvalidTokenOrAccountAlreadyActivated" },
        { ErrorCodes.TooFrequentActivationAttempt, "TooFrequentActivationAttempt" },

        // External dependencies
        { ErrorCodes.ExternalServiceUnavailable, "ExternalServiceUnavailable" },
        
        // Email-related
        { ErrorCodes.EmailSendFailed, "EmailSendFailed" }
    };

    public static string GetKey(int code)
    {
        return _keys.TryGetValue(code, out var key) ? key : "UnknownError";
    }
}