namespace SmartRetail360.Shared.Enums;

public enum LogEventType
{
    RegisterSuccess,
    RegisterFailure,
    LoginSuccess,
    LoginFailure,
    AccountActivateSuccess,
    AccountActivateFailure,
    EmailSendSuccess,
    EmailSendFailure,
    DataUpload,
    CopilotQuery,
    ExportReport,
    InternalError
}