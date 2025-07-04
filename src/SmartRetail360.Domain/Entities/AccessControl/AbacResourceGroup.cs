using SmartRetail360.Domain.Interfaces;

namespace SmartRetail360.Domain.Entities.AccessControl;

public class AbacResourceGroup : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<AbacResourceTypeGroupMap> ResourceTypes { get; set; } = new List<AbacResourceTypeGroupMap>();
}