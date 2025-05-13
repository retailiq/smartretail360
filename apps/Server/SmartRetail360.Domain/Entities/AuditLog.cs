using System.ComponentModel.DataAnnotations.Schema;

namespace SmartRetail360.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }
    public string Action { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } = false;
    public string TraceId { get; set; } = string.Empty;
    public string? DetailsJson { get; set; }
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    
    [NotMapped] // ✅ 不映射到数据库，只是给 AuditLogger 内部使用
    public Dictionary<string, string>? UnserializedDetails { get; set; }
}