using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Domain.Entities.AccessControl;

public class AbacAction  : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = DefaultActionType.None.GetEnumMemberValue();

    [NotMapped]
    public DefaultActionType NameEnum
    {
        get => Name.ToEnumFromMemberValue<DefaultActionType>();
        set => Name = value.GetEnumMemberValue();
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<AbacPolicy> Policies { get; set; } = new List<AbacPolicy>();
}