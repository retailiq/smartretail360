namespace SmartRetail360.Infrastructure.AuditLogging;

public class AuditContext
{
    public required string Action { get; init; }
    public required bool IsSuccess { get; init; }

    public string? Email { get; init; }
    public string? Error { get; init; }
    public string? Template { get; init; }
    public string? Reason { get; init; }
    public string? ReasonMessage { get; init; }

    public Dictionary<string, string>? Extra { get; init; }
}