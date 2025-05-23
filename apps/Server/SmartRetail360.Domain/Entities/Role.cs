using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Domain.Entities;

public class Role : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = StringCaseConverter.ToSnakeCase(nameof(SystemRoleType.Member));
    [NotMapped]
    public SystemRoleType NameEnum
    {
        get => Enum.Parse<SystemRoleType>(StringCaseConverter.ToPascalCase(Name));
        set => Name = StringCaseConverter.ToSnakeCase(value.ToString());
    }
    public string Description { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}