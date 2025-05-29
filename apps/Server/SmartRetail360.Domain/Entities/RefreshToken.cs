using System.ComponentModel.DataAnnotations.Schema;
using SmartRetail360.Domain.Interfaces;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Domain.Entities;

public class RefreshToken : IHasCreatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public Guid RoleId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public string CreatedByIp { get; set; } = string.Empty;
    public string? RevokedByIp { get; set; }
    public string ReasonRevoked { get; set; } = RefreshTokenRevokeReason.None.GetEnumMemberValue();

    [NotMapped]
    public RefreshTokenRevokeReason ReasonRevokedEnum
    {
        get => ReasonRevoked.ToEnumFromMemberValue<RefreshTokenRevokeReason>();
        set => ReasonRevoked = value.GetEnumMemberValue();
    }

    public bool IsActive => RevokedAt == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}