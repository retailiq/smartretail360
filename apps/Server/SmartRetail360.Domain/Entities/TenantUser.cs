using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Domain.Entities;

public class TenantUser : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public bool IsActive { get; set; } = false;                 
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow; 
    public DateTime? DeactivatedAt { get; set; } = null;
    public Guid? DeactivatedBy { get; set; } = null;                  
    public string DeactivationReason { get; set; } = StringCaseConverter.ToSnakeCase(nameof(AccountBanReason.None));
    [NotMapped]
    public AccountBanReason DeactivationReasonEnum
    {
        get => Enum.Parse<AccountBanReason>(StringCaseConverter.ToPascalCase(DeactivationReason));
        set => DeactivationReason = StringCaseConverter.ToSnakeCase(value.ToString());
    }
    public string TraceId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid? UpdatedBy { get; set; } = null;
    public DateTime? DeletedAt { get; set; } = null;
}