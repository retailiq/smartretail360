using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Domain.Entities;

public class Tenant : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; } 
    public string? Slug { get; set; } 
    public string? Industry { get; set; } = string.Empty;
    public int? Size { get; set; }
    public string? LogoUrl { get; set; } = string.Empty;
    public string Status { get; set; } = AccountStatus.PendingVerification.GetEnumMemberValue();
    [NotMapped]
    public AccountStatus StatusEnum
    {
        get => Status.ToEnumFromMemberValue<AccountStatus>();
        set => Status = value.GetEnumMemberValue();
    }
    public string Plan { get; set; } = AccountPlan.Free.GetEnumMemberValue();
    [NotMapped]
    public AccountPlan PlanEnum
    {
        get => Plan.ToEnumFromMemberValue<AccountPlan>();
        set => Plan = value.GetEnumMemberValue();
    }
    public string TraceId { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public Guid? LastUpdatedBy { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; } 
    public bool IsActive { get; set; } = true;
    public DateTime? DeactivatedAt { get; set; } 
    public Guid? DeactivatedBy { get; set; } 
    public string DeactivationReason { get; set; } = AccountBanReason.None.GetEnumMemberValue();
    [NotMapped]
    public AccountBanReason DeactivationReasonEnum
    {
        get => DeactivationReason.ToEnumFromMemberValue<AccountBanReason>();
        set => DeactivationReason = value.GetEnumMemberValue();
    }
}