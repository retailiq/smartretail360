namespace SmartRetail360.Shared.Logging;

public class LogContext
{
    public string? Email { get; init; }
    public string? Reason { get; init; }
    public string? ErrorStack { get; init; }
    public string? Action { get; set; }
    public bool IsSuccess { get; set; }
}