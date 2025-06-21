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
    public const string OAuthLogin = "OAUTH_LOGIN";
    public const string CredentialsLogin = "CREDENTIALS_LOGIN";
    public const string ConfirmTenantLogin = "CONFIRM_TENANT_LOGIN";
    public const string AbacAccessAttempt = "ABAC_ACCESS_ATTEMPT";
    public const string RefreshToken = "REFRESH_TOKEN";
    public const string ValidateToken = "VALIDATE_TOKEN";
    public const string Logout = "LOGOUT";
    public const string ValidateAccessToken = "VALIDATE_ACCESS_TOKEN";
    public const string EvaluateAbacPolicy = "EVALUATE_ABAC_POLICY";
    public const string GetAbacResolver = "GET_ABAC_RESOLVER";
    public const string GetAbacResource = "GET_ABAC_RESOURCE";
    public const string UpdateAbacPolicy = "UPDATE_ABAC_POLICY";
    public const string GetAbacPolicies = "GET_ABAC_POLICIES";

    // ===== Email & Notification =====
    public const string MatchEmailStrategy = "MATCH_EMAIL_STRATEGY";
    public const string MatchEmailTemplate = "MATCH_EMAIL_TEMPLATE";
    
    // ===== User Profile =====
    public const string UpdateUserBasicProfile = "UPDATE_USER_BASIC_PROFILE";
    
    // ===== Log =====
    public const string WriteAuditLog = "WRITE_AUDIT_LOG";
}