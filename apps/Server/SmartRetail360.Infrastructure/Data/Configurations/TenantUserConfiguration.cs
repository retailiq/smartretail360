using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Data.Configurations;

public class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> entity)
    {
        entity.ToTable("tenant_users");

        // Primary key
        entity.HasKey(e => e.Id);

        // Indexes
        entity.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
        entity.HasIndex(e => e.TraceId);
        entity.HasIndex(e => e.DeletedAt);
        entity.HasIndex(e => e.CreatedAt);
        entity.HasIndex(e => e.IsActive);
        entity.HasIndex(e => e.RoleId);
        entity.HasIndex(e => e.DeactivatedAt);

        // Required relationships
        entity.Property(e => e.TenantId).IsRequired();
        entity.Property(e => e.UserId).IsRequired();
        entity.Property(e => e.RoleId).IsRequired();

        // Soft ban
        entity.Property(e => e.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        entity.Property(e => e.DeactivationReason)
            .HasMaxLength(64)
            .HasDefaultValue(StringCaseConverter.ToSnakeCase(nameof(AccountBanReason.None)))
            .IsRequired();

        entity.Property(e => e.DeactivatedAt)
            .HasColumnType("timestamp with time zone");

        // Lifecycle
        entity.Property(e => e.JoinedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        entity.Property(e => e.TraceId)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.CreatedBy)
            .IsRequired();

        entity.Property(e => e.UpdatedAt)
            .IsRequired();

        entity.Property(e => e.DeletedAt)
            .HasColumnType("timestamp with time zone");
    }
}