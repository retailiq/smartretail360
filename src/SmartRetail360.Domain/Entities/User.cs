using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Domain.Entities;

public class User : IHasCreatedAt, IHasUpdatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? CountryCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public string Locale { get; set; } = LocaleType.En.GetEnumMemberValue();

    [NotMapped]
    public LocaleType LocaleEnum
    {
        get => Locale.ToEnumFromMemberValue<LocaleType>();
        set => Locale = value.GetEnumMemberValue();
    }

    public string Status { get; set; } = AccountStatus.PendingVerification.GetEnumMemberValue();

    [NotMapped]
    public AccountStatus StatusEnum
    {
        get => Status.ToEnumFromMemberValue<AccountStatus>();
        set => Status = value.GetEnumMemberValue();
    }

    public bool IsEmailVerified { get; set; } 
    public bool IsFirstLogin { get; set; } = true;
    public string TraceId { get; set; } = string.Empty;
    public Guid? LastUpdatedBy { get; set; }
    public DateTime? LastEmailSentAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsSystemAccount { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public Guid? DeactivatedBy { get; set; }
    public string DeactivationReason { get; set; } = AccountBanReason.None.GetEnumMemberValue();

    [NotMapped]
    public AccountBanReason DeactivationReasonEnum
    {
        get => DeactivationReason.ToEnumFromMemberValue<AccountBanReason>();
        set => DeactivationReason = value.GetEnumMemberValue();
    }

    public ICollection<OAuthAccount> OAuthAccounts { get; set; } = new List<OAuthAccount>();
}