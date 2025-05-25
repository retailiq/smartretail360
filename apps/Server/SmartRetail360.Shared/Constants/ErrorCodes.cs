namespace SmartRetail360.Shared.Constants;

public static class ErrorCodes
{
    // General errors
    public const int InternalServerError = 10000;
    public const int DatabaseUnavailable = 10001;
    public const int DuplicateRegisterAttempt = 10002;

    // Auth
    public const int InvalidPassword = 5001;
    public const int InvalidToken = 5002;
    public const int TokenExpired = 5003;
    public const int TokenAlreadyUsed = 5004;
    public const int TokenRevoked = 5005;
    public const int HasPendingActivationEmail = 5006;

    // Account
    public const int AccountLocked = 6001;
    public const int AccountNotFound = 6002;
    public const int AccountAlreadyActivated = 6003;
    public const int AccountExistsButNotActivated = 6004;
    public const int TooFrequentActivationAttempt = 6005;

    // Email & Notification
    public const int EmailSendFailed = 7001;
    public const int EmailExists = 7002;
    public const int TooFrequentEmailRequest = 7003;
    public const int EmailTemplateNotFound = 7004;
    public const int EmailStrategyNotFound = 7005;
    
    // Unknown
    public const int UnknownError = 9999;
}