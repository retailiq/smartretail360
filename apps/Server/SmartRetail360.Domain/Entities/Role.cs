using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Domain.Entities;

public class Role : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = SystemRoleType.Member.GetEnumMemberValue();
    [NotMapped]
    public SystemRoleType NameEnum
    {
        get => Name.ToEnumFromMemberValue<SystemRoleType>();
        set => Name = value.GetEnumMemberValue();
    }
    public bool IsSystemRole { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}