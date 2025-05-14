namespace SmartRetail360.Shared.Constants;

public static class LogReasons
{
    // General operation failures
    public const string LockNotAcquired = "LOCK_NOT_ACQUIRED";
    public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
    public const string InvalidEmailFormat = "INVALID_EMAIL_FORMAT";
    public const string PasswordTooWeak = "PASSWORD_TOO_WEAK";
    public const string DatabaseSaveFailed = "DATABASE_SAVE_FAILED";
    public const string MissingRequiredFields = "MISSING_REQUIRED_FIELDS";
    public const string UnexpectedError = "UNEXPECTED_ERROR";
    public const string InvalidRequest = "INVALID_REQUEST";

    // Authentication / Token
    public const string InvalidToken = "INVALID_TOKEN";
    public const string TokenExpired = "TOKEN_EXPIRED";
    public const string TokenGenerationError = "TOKEN_GENERATION_ERROR";

    // Tenant-specific
    public const string TenantAlreadyExists = "TENANT_ALREADY_EXISTS";
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
    
    // Email request
    public const string EmailSendFailed = "EMAIL_SEND_FAILED";

    // Optional: Map to description
    public static readonly Dictionary<string, string> Descriptions = new()
    {
        { LockNotAcquired, "Lock not acquired" },
        { EmailAlreadyExists, "AdminEmail already exists" },
        { InvalidEmailFormat, "Invalid email format" },
        { PasswordTooWeak, "Password does not meet complexity rules" },
        { DatabaseSaveFailed, "Failed to save tenant to database" },
        { MissingRequiredFields, "One or more required fields are missing" },
        { UnexpectedError, "Unexpected internal error occurred" },
        { InvalidRequest, "The request payload is invalid" },

        { InvalidToken, "Provided token is invalid" },
        { TokenExpired, "Token has expired" },
        { TokenGenerationError, "Failed to generate token" },

        { TenantAlreadyExists, "Tenant with same slug or email already exists" },
        { TenantNotFound, "Target tenant was not found" },

        { ActivationEmailFailed, "Failed to send activation email" },
        { EmailTokenGenerationError, "Email verification token generation failed" },

        { RedisUnavailable, "Redis is unavailable or misconfigured" },
        { PasswordHashingError, "Password hashing failed" },
        { ExternalServiceUnavailable, "External service is unavailable" },

        { Unauthorized, "User is not authorized to perform this action" },
        { Forbidden, "User does not have permission" },

        { RateLimited, "Too many requests in short time" },
        { TooFrequentEmailRequest, "Email requests are too frequent" },
        
        { EmailSendFailed, "Failed to send email" },
    };
}