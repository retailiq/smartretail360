using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;

namespace SmartRetail360.Infrastructure.Data.Configurations;

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

        entity.Property(rt => rt.CreatedByIp)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(rt => rt.RevokedByIp)
            .HasMaxLength(64);

        entity.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(256);

        entity.Property(rt => rt.ReasonRevoked)
            .HasMaxLength(64)
            .IsRequired();
    }
}