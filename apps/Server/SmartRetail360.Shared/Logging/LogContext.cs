namespace SmartRetail360.Shared.Logging;

public class LogContext
{
    public string LogId { get; set; } = Guid.NewGuid().ToString();
    public string? Reason { get; init; }
}