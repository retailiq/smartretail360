using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Domain.Entities;

public class TenantUser : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; } 
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }
    public bool IsActive { get; set; }  
    public bool IsDefault { get; set; } 
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow; 
    public DateTime? DeactivatedAt { get; set; } 
    public Guid? DeactivatedBy { get; set; }                  
    public string DeactivationReason { get; set; } = AccountBanReason.None.GetEnumMemberValue();
    [NotMapped]
    public AccountBanReason DeactivationReasonEnum
    {
        get => DeactivationReason.ToEnumFromMemberValue<AccountBanReason>();
        set => DeactivationReason = value.GetEnumMemberValue();
    }
    public string TraceId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid? UpdatedBy { get; set; } 
    public DateTime? DeletedAt { get; set; }
}