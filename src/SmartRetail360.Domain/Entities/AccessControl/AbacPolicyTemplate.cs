using SmartRetail360.Domain.Interfaces;

namespace SmartRetail360.Domain.Entities.AccessControl;

public class AbacPolicyTemplate : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TemplateName { get; set; } = null!;
    public string ResourceType { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string RuleJson { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}