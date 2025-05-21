using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string LogId { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }
    public string Action { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } = false;
    public string TraceId { get; set; } = string.Empty;
    public string? DetailsJson { get; set; }
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    public LogLevel Level { get; set; } = LogLevel.Information;
    public string? SourceModule { get; set; }
    public string? Category { get; set; }
    
    [NotMapped]
    public Dictionary<string, string>? UnserializedDetails { get; set; }
}