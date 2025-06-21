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
    public const string AbacTenantUserRecordNotFound = "TENANT_USER_RECORD_NOT_FOUND";
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
    public const string RefreshTokenMissing = "REFRESH_TOKEN_MISSING";
    public const string RefreshTokenReplayDetected = "REFRESH_TOKEN_REPLAY_DETECTED";
    public const string AccessDeniedByPolicy = "ACCESS_DENIED_BY_POLICY";
    public const string AbacPolicyNotFound = "ABAC_POLICY_NOT_FOUND";
    public const string InvalidAbacPolicyRule = "INVALID_ABAC_POLICY_RULE";
    public const string TokenMissing = "TOKEN_MISSING";
    public const string RefreshTokenExpired = "REFRESH_TOKEN_EXPIRED";
    public const string RefreshTokenRevoked = "REFRESH_TOKEN_REVOKED";
    public const string TokenValidationFailed = "TOKEN_VALIDATION_FAILED";
    public const string AccountNotActivatedOrInvitationPending = "ACCOUNT_NOT_ACTIVATED_OR_INVITATION_PENDING";
    public const string PolicyEvaluationFailed = "POLICY_EVALUATION_FAILED";
    public const string PolicyNotFoundOrDisabled = "POLICY_NOT_FOUND_OR_DISABLED";
    public const string AbacResolverNotFound = "ABAC_RESOLVER_NOT_FOUND";
    public const string AbacResolverExecutionFailed = "ABAC_RESOLVER_EXECUTION_FAILED";
    public const string AbacResourceIdMissing = "ABAC_RESOURCE_ID_MISSING";
    public const string AbacUserTenantRoleNotFound = "ABAC_USER_TENANT_ROLE_NOT_FOUND";
    public const string AbacPolicyUnchanged = "ABAC_POLICY_UNCHANGED";
    public const string AbacPolicyDisabled = "ABAC_POLICY_DISABLED";
    public const string AbacPolicyHasBeenReplaced = "ABAC_POLICY_HAS_BEEN_REPLACED";

    // Account
    public const string AccountAlreadyExists = "ACCOUNT_ALREADY_EXISTS";
    public const string AccountAlreadyActivated = "ACCOUNT_ALREADY_ACTIVATED";
    public const string AccountExistsButNotActivated = "ACCOUNT_EXISTS_BUT_NOT_ACTIVATED";
    public const string AccountNotFound = "ACCOUNT_NOT_FOUND";
    public const string TooFrequentActivationAttempt = "TOO_FREQUENT_ACTIVATION_ATTEMPT";
    public const string TenantNotFound = "TENANT_NOT_FOUND";

    // Users
    public const string UserNotExists = "USER_NOT_EXISTS";
    public const string TenantUserRecordNotFound = "TENANT_USER_RECORD_NOT_FOUND";
    public const string BasicProfileUnchanged = "BASIC_PROFILE_UNCHANGED";

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