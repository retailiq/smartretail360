namespace SmartRetail360.Shared.Constants;

public static class ErrorCodes
{
    // General errors
    public const int InternalServerError = 10000;
    public const int DatabaseUnavailable = 10001;
    public const int ValidationFailed = 10002;
    public const int UnsupportedEmailTemplate = 10003;
    public const int DuplicateRegisterAttempt = 10004;

    // Authorization / Access control
    public const int Unauthorized = 401;
    public const int Forbidden = 403;

    // Tenant / Account-related errors
    public const int EmailExists = 5001;
    public const int TenantNotFound = 5002;
    public const int InvalidPassword = 5003;
    public const int AccountLocked = 5004;
    public const int AccountNotFound = 5005;
    public const int InvalidToken = 5006;
    public const int TokenExpired = 5007;
    public const int AccountAlreadyActivated = 5008;
    public const int AccountExists = 5009;

    // Operation restrictions
    public const int RateLimitExceeded = 6001;
    public const int OperationNotAllowed = 6002;
    public const int TooFrequentEmailRequest = 6003;
    public const int InvalidTokenOrAccountAlreadyActivated = 6004;

    // External dependencies
    public const int ExternalServiceUnavailable = 7001;
    
    // Email-related
    public const int EmailSendFailed = 8001;
}