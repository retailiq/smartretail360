using Microsoft.Extensions.Logging;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Shared.Logging;

public class AuditContext
{
    public required string LogId { get; init; }
    public required string Action { get; init; }
    public required bool IsSuccess { get; init; }
    public LogLevel Level { get; init; } = LogLevel.Information;
    public string? SourceModule { get; init; }
    public string? Email { get; init; }
    public string? ErrorStack { get; init; }
    public string? Reason { get; init; }
    public Guid? UserId { get; init; }
    public Guid? TenantId { get; init; }
    public string? Category { get; init; }
}