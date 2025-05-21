namespace SmartRetail360.Shared.Constants;

public static class ErrorCodes
{
    // General errors
    public const int InternalServerError = 10000;
    public const int DatabaseUnavailable = 10001;
    public const int UnsupportedEmailTemplate = 10002;
    public const int DuplicateRegisterAttempt = 10003;

    // Auth
    public const int InvalidPassword = 5001;
    public const int InvalidToken = 5002;
    public const int TokenExpired = 5003;

    // Account
    public const int TenantNotFound = 6001;
    public const int AccountLocked = 6002;
    public const int AccountNotFound = 6003;
    public const int AccountAlreadyActivated = 6004;
    public const int AccountExistsButNotActivated = 6005;
    public const int InvalidTokenOrAccountAlreadyActivated = 6006;
    public const int TooFrequentActivationAttempt = 6007;

    // Email & Notification
    public const int EmailSendFailed = 7001;
    public const int EmailExists = 7002;
    public const int TooFrequentEmailRequest = 7003;
    
    // Unknown
    public const int UnknownError = 9999;
}