using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Domain.Entities.AccessControl;

public class AbacEnvironment  : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = DefaultEnvironmentType.Default.GetEnumMemberValue();

    [NotMapped]
    public DefaultEnvironmentType NameEnum
    {
        get => Name.ToEnumFromMemberValue<DefaultEnvironmentType>();
        set => Name = value.GetEnumMemberValue();
    }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AbacPolicy> Policies { get; set; } = new List<AbacPolicy>();
}