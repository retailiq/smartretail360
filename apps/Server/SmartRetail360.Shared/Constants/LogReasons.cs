namespace SmartRetail360.Shared.Constants;

public static class LogReasons
{
    // General operation failures
    public const string LockNotAcquired = "LOCK_NOT_ACQUIRED";

    // Auth
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string TokenDeserializationFailed = "TOKEN_DESERIALIZATION_FAILED";
    public const string RoleDeserializationFailed = "ROLE_DESERIALIZATION_FAILED";

    // Account
    public const string InvalidTokenOrAccountAlreadyActivated = "INVALID_TOKEN_OR_ACCOUNT_ALREADY_ACTIVATED";
    public const string AccountAlreadyExists = "ACCOUNT_ALREADY_EXISTS";
    public const string AccountAlreadyActivated = "ACCOUNT_ALREADY_ACTIVATED";
    public const string AccountExistsButNotActivated = "ACCOUNT_EXISTS_BUT_NOT_ACTIVATED";
    public const string AccountNotFound = "ACCOUNT_NOT_FOUND";
    public const string TooFrequentActivationAttempt = "TOO_FREQUENT_ACTIVATION_ATTEMPT";

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
}