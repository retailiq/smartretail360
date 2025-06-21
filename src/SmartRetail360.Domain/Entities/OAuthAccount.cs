using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Domain.Entities;

public class OAuthAccount : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = Guid.Empty;
    public User? User { get; set; } 
    public string Provider { get; set; } = OAuthProvider.None.GetEnumMemberValue();

    [NotMapped]
    public OAuthProvider ProviderEnum
    {
        get => Provider.ToEnumFromMemberValue<OAuthProvider>();
        set => Provider = value.GetEnumMemberValue();
    }

    public string ProviderUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = AccountStatus.Active.GetEnumMemberValue();

    [NotMapped]
    public AccountStatus StatusEnum
    {
        get => Status.ToEnumFromMemberValue<AccountStatus>();
        set => Status = value.GetEnumMemberValue();
    }

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