using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Persistence.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.ToTable("refresh_tokens");

        entity.HasKey(rt => rt.Id);

        entity.HasIndex(rt => rt.UserId);
        entity.HasIndex(rt => rt.Token).IsUnique();
        entity.HasIndex(rt => rt.ExpiresAt);
        entity.HasIndex(e => new { e.TenantId, e.UserId });

        entity.Property(e => e.TenantId)
            .IsRequired();

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(rt => rt.ExpiresAt)
            .IsRequired();

        entity.Property(rt => rt.CreatedAt)
            .IsRequired();

        entity.Property(rt => rt.Token)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(rt => rt.RoleId)
            .IsRequired();
        
        entity.Property(rt => rt.RoleName)
            .IsRequired()
            .HasDefaultValue(SystemRoleType.Member.GetEnumMemberValue());

        entity.Property(rt => rt.Email)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(rt => rt.UserName)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(rt => rt.Locale)
            .HasMaxLength(16)
            .IsRequired();

        entity.Property(rt => rt.TraceId)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(rt => rt.CreatedByIp)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(rt => rt.RevokedByIp)
            .HasMaxLength(64);

        entity.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(256);
        
        entity.Property(rt => rt.Env)
            .HasMaxLength(16)
            .IsRequired()
            .HasDefaultValue(DefaultEnvironmentType.Default.GetEnumMemberValue());

        entity.Property(rt => rt.ReasonRevoked)
            .HasMaxLength(64)
            .IsRequired()
            .HasDefaultValue(RefreshTokenRevokeReason.None.GetEnumMemberValue());
    }
}