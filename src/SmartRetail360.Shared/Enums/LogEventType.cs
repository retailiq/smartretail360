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
    WriteLogFailure,
    DataUpload,
    CopilotQuery,
    ExportReport,
    InternalError
}