namespace SmartRetail360.Shared.Constants;

public static class LogReasons
{
    // General operation failures
    public const string LockNotAcquired = "LOCK_NOT_ACQUIRED";
    public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
    public const string InvalidEmailFormat = "INVALID_EMAIL_FORMAT";
    public const string PasswordTooWeak = "PASSWORD_TOO_WEAK";
    public const string MissingRequiredFields = "MISSING_REQUIRED_FIELDS";
    public const string UnexpectedError = "UNEXPECTED_ERROR";
    public const string InvalidRequest = "INVALID_REQUEST";

    // Authentication / Token
    public const string InvalidToken = "INVALID_TOKEN";
    public const string TokenExpired = "TOKEN_EXPIRED";
    public const string TokenGenerationError = "TOKEN_GENERATION_ERROR";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";

    // Account
    public const string InvalidTokenOrAccountAlreadyActivated = "INVALID_TOKEN_OR_ACCOUNT_ALREADY_ACTIVATED";
    
    // Tenant-specific
    public const string TenantAccountAlreadyExists = "TENANT_ACCOUNT_ALREADY_EXISTS";
    public const string TenantAccountAlreadyActivated = "TENANT_ACCOUNT_ALREADY_ACTIVATED";
    public const string TenantAccountExistsButNotActivated = "TENANT_ACCOUNT_EXISTS_BUT_NOT_ACTIVATED";
    public const string TenantNotFound = "TENANT_NOT_FOUND";

    // Email & Notification
    public const string ActivationEmailFailed = "ACTIVATION_EMAIL_FAILED";
    public const string EmailTokenGenerationError = "EMAIL_TOKEN_GENERATION_ERROR";

    // Infrastructure / System
    public const string RedisUnavailable = "REDIS_UNAVAILABLE";
    public const string PasswordHashingError = "PASSWORD_HASHING_ERROR";
    public const string ExternalServiceUnavailable = "EXTERNAL_SERVICE_UNAVAILABLE";

    // Access control
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";

    // Rate limiting
    public const string RateLimited = "RATE_LIMITED";
    public const string TooFrequentEmailRequest = "TOO_FREQUENT_EMAIL_REQUEST";
    public const string TooFrequentActivationAttempt = "TOO_FREQUENT_ACTIVATION_ATTEMPT";
    
    // Email request
    public const string EmailSendFailed = "EMAIL_SEND_FAILED";
    
    // AWS SQS
    public const string SendSqsMessageFailed = "SEND_SQS_MESSAGE_FAILED";
    
    // Database operations
    public const string DatabaseSaveFailed = "DATABASE_SAVE_FAILED";
    public const string DatabaseRetrievalFailed = "DATABASE_RETRIEVAL_FAILED";
}