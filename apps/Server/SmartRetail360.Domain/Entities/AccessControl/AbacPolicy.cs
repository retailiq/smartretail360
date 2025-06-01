using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Domain.Entities.AccessControl;

public class AbacPolicy : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ResourceTypeId { get; set; }
    public Guid ActionId { get; set; }
    public Guid EnvironmentId { get; set; }
    public Guid TenantId { get; set; } = Guid.Empty;
    public SystemRoleType? AppliesToRole { get; set; }
    
    public int VersionNumber { get; set; } = 1;

    [NotMapped] public string Version => $"v{VersionNumber}";

    public string RuleJson { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;

    public Guid UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Guid? BasePolicyId { get; set; } // ✅ 模板继承支持
    public AbacPolicy? BasePolicy { get; set; }
    
    public AbacResourceType ResourceType { get; set; } = default!;
    public AbacAction Action { get; set; } = default!;
    public AbacEnvironment Environment { get; set; } = default!;
}