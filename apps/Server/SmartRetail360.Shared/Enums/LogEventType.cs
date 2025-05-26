namespace SmartRetail360.Shared.Enums;

public enum LogEventType
{
    RegisterUserSuccess,
    RegisterUserFailure,
    InviteUserSuccess,
    InviteUserFailure,
    UserLoginSuccess,
    UserLoginFailure,
    AccountActivateSuccess,
    AccountActivateFailure,
    EmailSendSuccess,
    EmailSendFailure,
    WriteLogFailure,
    GeneralError,
    ConfirmTenantLoginFailure,
    ConfirmTenantLoginSuccess,
}