namespace SmartRetail360.Shared.Constants;

public static class LogActions
{
    // ===== Account =====
    public const string AccountRegister = "ACCOUNT_REGISTER";
    public const string AccountAdding = "ACCOUNT_ADDING";
    public const string AccountActivate = "ACCOUNT_ACTIVATE";
    public const string AccountRegistrationActivateEmailSend = "ACCOUNT_REGISTRATION_ACTIVATE_EMAIL_SEND";
    public const string AccountRegistrationActivateEmailReSend = "ACCOUNT_REGISTRATION_ACTIVATE_EMAIL_RESEND";
    
    // ===== Auth =====
    public const string UserResetPassword = "USER_RESET_PASSWORD";
    public const string UserLogin = "USER_LOGIN";
    
    // ===== Redis =====
    public const string GenerateAccountLockFailed = "GENERATE_ACCOUNT_LOCK_FAILED";
    
    // ===== Email & Notification =====
    public const string MatchEmailStrategy = "MATCH_EMAIL_STRATEGY";
    public const string MatchEmailTemplate = "MATCH_EMAIL_TEMPLATE";
    
    // ===== Log =====
    public const string WriteAuditLog = "WRITE_AUDIT_LOG";
    
}