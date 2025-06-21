using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Domain.Entities.AccessControl;

public class AbacResourceType : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = DefaultResourceType.None.GetEnumMemberValue();

    [NotMapped]
    public DefaultResourceType NameEnum
    {
        get => Name.ToEnumFromMemberValue<DefaultResourceType>();
        set => Name = value.GetEnumMemberValue();
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid? GroupId { get; set; } 
    public AbacResourceGroup? Group { get; set; } 
    
    public ICollection<AbacResourceTypeGroupMap> Groups { get; set; } = new List<AbacResourceTypeGroupMap>();
}