namespace SmartRetail360.Shared.Constants;

public static class LogReasons
{
    // General operation failures
    public const string LockNotAcquired = "LOCK_NOT_ACQUIRED";
    public const string RoleListNotFound = "ROLE_LIST_NOT_FOUND";

    // Auth
    public const string PasswordEmailMismatch = "PASSWORD_EMAIL_MISMATCH";
    public const string TokenDeserializationFailed = "TOKEN_DESERIALIZATION_FAILED";
    public const string RoleDeserializationFailed = "ROLE_DESERIALIZATION_FAILED";
    public const string TokenAlreadyUsed = "TOKEN_ALREADY_USED";
    public const string InvalidToken = "INVALID_TOKEN";
    public const string TokenExpired = "TOKEN_EXPIRED";
    public const string TokenRevoked = "TOKEN_REVOKED";
    public const string HasPendingActivationEmail = "HAS_PENDING_ACTIVATION_EMAIL";
    public const string AccountNotActivated = "ACCOUNT_NOT_ACTIVATED";
    public const string TenantUserRecordNotFound = "TENANT_USER_RECORD_NOT_FOUND";
    public const string AccountLockedDueToLoginFailures = "ACCOUNT_LOCKED_DUE_TO_LOGIN_FAILURES";
    public const string AccountLocked = "ACCOUNT_LOCKED";
    public const string AllTenantsDisabled = "ALL_TENANTS_DISABLED";
    public const string TenantDisabled = "TENANT_DISABLED";
    public const string TenantUserDisabled = "TENANT_USER_DISABLED";
    public const string AccountSuspended = "ACCOUNT_SUSPENDED";
    public const string AccountDeleted = "ACCOUNT_DELETED";
    public const string AccountBanned = "ACCOUNT_BANNED";
    public const string TokenNotFound = "TOKEN_NOT_FOUND";
    public const string RefreshTokenCreationFailed = "REFRESH_TOKEN_CREATION_FAILED";
    public const string OAuthHandlerNotFound = "OAUTH_HANDLER_NOT_FOUND";
    public const string OAuthUserProfileFetchFailed = "OAUTH_USER_PROFILE_FETCH_FAILED";
    public const string OAuthUserProfileNotExists = "OAUTH_USER_PROFILE_NOT_EXISTS";
    public const string RefreshTokenNotFound = "REFRESH_TOKEN_NOT_FOUND";
    public const string RefreshTokenReplayDetected = "REFRESH_TOKEN_REPLAY_DETECTED";

    // Account
    public const string AccountAlreadyExists = "ACCOUNT_ALREADY_EXISTS";
    public const string AccountAlreadyActivated = "ACCOUNT_ALREADY_ACTIVATED";
    public const string AccountExistsButNotActivated = "ACCOUNT_EXISTS_BUT_NOT_ACTIVATED";
    public const string AccountNotFound = "ACCOUNT_NOT_FOUND";
    public const string TooFrequentActivationAttempt = "TOO_FREQUENT_ACTIVATION_ATTEMPT";
    public const string TenantNotFound = "TENANT_NOT_FOUND";

    // Email & Notification
    public const string TooFrequentEmailRequest = "TOO_FREQUENT_EMAIL_REQUEST";
    public const string EmailSendFailed = "EMAIL_SEND_FAILED";
    public const string EmailStrategyNotFound = "EMAIL_STRATEGY_NOT_FOUND";
    public const string EmailTemplateNotFound = "EMAIL_TEMPLATE_NOT_FOUND";

    // AWS SQS
    public const string SendSqsMessageFailed = "SEND_SQS_MESSAGE_FAILED";

    // Database
    public const string DatabaseSaveFailed = "DATABASE_SAVE_FAILED";
    public const string DatabaseRetrievalFailed = "DATABASE_RETRIEVAL_FAILED";
    public const string RedisOperateFailed = "REDIS_OPERATE_FAILED";

    // OAuth
    public const string OAuthRedirectUriUnauthorized = "OAUTH_REDIRECT_URI_UNAUTHORIZED";
    public const string FacebookTokenExchangeFailed = "FACEBOOK_TOKEN_EXCHANGE_FAILED";
    public const string FacebookInvalidTokenResponse = "FACEBOOK_INVALID_TOKEN_RESPONSE_PAYLOAD_NULL_OR_EMPTY";
    public const string FacebookProfileFetchFailed = "FACEBOOK_PROFILE_FETCH_FAILED";
    public const string FacebookInvalidUserProfile = "FACEBOOK_INVALID_USER_PROFILE_PAYLOAD_NULL_OR_EMPTY";
    public const string GoogleTokenExchangeFailed = "GOOGLE_TOKEN_EXCHANGE_FAILED";
    public const string GoogleInvalidTokenResponse = "GOOGLE_INVALID_TOKEN_RESPONSE";
    public const string GoogleProfileFetchFailed = "GOOGLE_PROFILE_FETCH_FAILED";
    public const string GoogleInvalidUserProfile = "GOOGLE_INVALID_USER_PROFILE";
    public const string MicrosoftTokenExchangeFailed = "MICROSOFT_TOKEN_EXCHANGE_FAILED";
    public const string MicrosoftInvalidTokenResponse = "MICROSOFT_INVALID_TOKEN_RESPONSE";
    public const string MicrosoftProfileFetchFailed = "MICROSOFT_PROFILE_FETCH_FAILED";
    public const string MicrosoftInvalidUserProfile = "MICROSOFT_INVALID_USER_PROFILE";
}