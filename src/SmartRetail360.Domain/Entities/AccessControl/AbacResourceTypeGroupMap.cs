using SmartRetail360.Domain.Interfaces;

namespace SmartRetail360.Domain.Entities.AccessControl;

public class AbacResourceTypeGroupMap : IHasCreatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid GroupId { get; set; }
    public AbacResourceGroup Group { get; set; } = null!;

    public Guid ResourceTypeId { get; set; }
    public AbacResourceType ResourceType { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}