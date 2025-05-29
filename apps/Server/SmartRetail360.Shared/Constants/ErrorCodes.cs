namespace SmartRetail360.Shared.Constants;

public static class ErrorCodes
{
    // General errors
    public const int None = 0;
    public const int InternalServerError = 10000;
    public const int DatabaseUnavailable = 10001;
    public const int DuplicateRegisterAttempt = 10002;
    public const int DuplicateLoginAttempt = 10003;

    // Auth
    public const int InvalidPassword = 5001;
    public const int InvalidToken = 5002;
    public const int TokenExpired = 5003;
    public const int TokenAlreadyUsed = 5004;
    public const int TokenRevoked = 5005;
    public const int HasPendingActivationEmail = 5006;
    public const int PasswordEmailMismatch = 5007;
    public const int AccountNotActivated = 5008;
    public const int AccountLocked = 5009;
    public const int AllTenantsDisabled = 5010;
    public const int TenantDisabled = 5011;
    public const int TenantUserDisabled = 5012;
    public const int AccountSuspended = 5013;
    public const int AccountDeleted = 5014;
    public const int AccountBanned = 5015;
    public const int TenantUserRecordNotFound = 5016;
    public const int TokenNotFound = 5017;
    public const int InvalidOAuthProvider = 5018;
    public const int OAuthUserProfileFetchFailed = 5019;
    public const int OAuthUserProfileNotExists = 5020;
    public const int RefreshTokenNotFound = 5021;

    // Account
    public const int AccountNotFound = 6002;
    public const int AccountAlreadyActivated = 6003;
    public const int AccountExistsButNotActivated = 6004;
    public const int TooFrequentActivationAttempt = 6005;
    public const int TenantNotFound = 6006;

    // Email & Notification
    public const int EmailSendFailed = 7001;
    public const int EmailExists = 7002;
    public const int TooFrequentEmailRequest = 7003;
    public const int EmailTemplateNotFound = 7004;
    public const int EmailStrategyNotFound = 7005;
    
    // Unknown
    public const int UnknownError = 9999;
}