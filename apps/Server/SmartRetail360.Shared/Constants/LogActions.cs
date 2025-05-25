namespace SmartRetail360.Shared.Constants;

public static class LogActions
{
    // ===== User Account =====
    public const string UserRegister = "USER_REGISTER";
    public const string UserInvite = "USER_INVITE";
    public const string UserRegistrationActivate = "USER_REGISTRATION_ACTIVATE";
    public const string UserInvitationActivate = "USER_INVITATION_ACTIVATE";
    public const string UserRegistrationActivationEmailSend = "USER_REGISTRATION_ACTIVATE_EMAIL_SEND";
    public const string UserRegistrationActivationEmailResend = "USER_REGISTRATION_ACTIVATE_EMAIL_RESEND";
    public const string UserInvitationActivationEmailSend = "USER_INVITATION_ACTIVATE_EMAIL_SEND";
    public const string UserInvitationActivationEmailResend = "USER_INVITATION_ACTIVATE_EMAIL_RESEND";
    
    // ===== Auth =====
    public const string UserResetPassword = "USER_RESET_PASSWORD";
    public const string UserLogin = "USER_LOGIN";
    
    // ===== Email & Notification =====
    public const string MatchEmailStrategy = "MATCH_EMAIL_STRATEGY";
    public const string MatchEmailTemplate = "MATCH_EMAIL_TEMPLATE";
    
    // ===== Log =====
    public const string WriteAuditLog = "WRITE_AUDIT_LOG";
    
}