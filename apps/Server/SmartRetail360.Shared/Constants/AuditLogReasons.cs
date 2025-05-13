namespace SmartRetail360.Shared.Constants;

public static class AuditLogReasons
{
    // General operation failures
    public const string LockNotAcquired = "LockNotAcquired";
    public const string EmailAlreadyExists = "EmailAlreadyExists";
    public const string InvalidEmailFormat = "InvalidEmailFormat";
    public const string PasswordTooWeak = "PasswordTooWeak";
    public const string DatabaseSaveFailed = "DatabaseSaveFailed";
    public const string MissingRequiredFields = "MissingRequiredFields";
    public const string UnexpectedError = "UnexpectedError";
    public const string InvalidRequest = "InvalidRequest";

    // Authentication / Token
    public const string InvalidToken = "InvalidToken";
    public const string TokenExpired = "TokenExpired";
    public const string TokenGenerationError = "TokenGenerationError";

    // Tenant-specific
    public const string TenantAlreadyExists = "TenantAlreadyExists";
    public const string TenantNotFound = "TenantNotFound";

    // Email & Notification
    public const string ActivationEmailFailed = "ActivationEmailFailed";
    public const string EmailTokenGenerationError = "EmailTokenGenerationError";

    // Infrastructure / System
    public const string RedisUnavailable = "RedisUnavailable";
    public const string PasswordHashingError = "PasswordHashingError";
    public const string ExternalServiceUnavailable = "ExternalServiceUnavailable";

    // Access control
    public const string Unauthorized = "Unauthorized";
    public const string Forbidden = "Forbidden";

    // Rate limiting
    public const string RateLimited = "RateLimited";
    public const string TooFrequentEmailRequest = "TooFrequentEmailRequest";

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
        { TooFrequentEmailRequest, "Email requests are too frequent" }
    };
}