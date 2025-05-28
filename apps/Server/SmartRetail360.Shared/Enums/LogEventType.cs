namespace SmartRetail360.Shared.Enums;

public enum LogEventType
{
    RegisterUserSuccess,
    RegisterUserFailure,
    InviteUserSuccess,
    InviteUserFailure,
    UserLoginSuccess,
    UserLoginFailure,
    OAuthLoginSuccess,
    OAuthLoginFailure,
    AccountActivateSuccess,
    AccountActivateFailure,
    EmailSendSuccess,
    EmailSendFailure,
    WriteLogFailure,
    GeneralError,
    RedisError,
    DatabaseError,
    ConfirmTenantLoginFailure,
    ConfirmTenantLoginSuccess,
}