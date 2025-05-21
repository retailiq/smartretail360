namespace SmartRetail360.Shared.Constants;

public static class LogActions
{
    // ===== Tenant Account =====
    public const string TenantRegister = "TENANT_REGISTER";
    public const string TenantAccountActivate = "TENANT_ACCOUNT_ACTIVATE";
    public const string TenantAccountActivateEmailReSend = "TENANT_ACCOUNT_ACTIVATE_EMAIL_RESEND";
    public const string TenantUpdateProfile = "TENANT_UPDATE_PROFILE";

    // ===== User Account =====
    public const string UserRegister = "USER_REGISTER";
    public const string UserAccountActivate = "USER_ACCOUNT_ACTIVATE";
    public const string UserAccountActivateEmailSend = "USER_ACCOUNT_ACTIVATE_EMAIL_SEND";
    public const string UserLogout = "USER_LOGOUT";
    public const string UserUpdateProfile = "USER_UPDATE_PROFILE";
    public const string UserDelete = "USER_DELETE";
    
    // ===== User Auth =====
    public const string UserResetPassword = "USER_RESET_PASSWORD";
    public const string UserLogin = "USER_LOGIN";
    
    // ===== Redis =====
    public const string GenerateAccountLockFailed = "GENERATE_ACCOUNT_LOCK_FAILED";
    
    // ===== Email & Notification =====
    public const string SendEmailFailed = "SEND_EMAIL_FAILED";
    
    // ===== Log =====
    public const string WriteAuditLog = "WRITE_AUDIT_LOG";
    
}