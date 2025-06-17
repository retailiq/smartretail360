using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Persistence.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> entity)
    {
        entity.ToTable("tenants");

        // Primary key
        entity.HasKey(e => e.Id);

        // Indexes
        entity.HasIndex(e => e.DeletedAt);
        entity.HasIndex(e => e.Slug).IsUnique();
        entity.HasIndex(e => e.TraceId);
        entity.HasIndex(e => e.CreatedAt);
        entity.HasIndex(e => e.IsActive);
        entity.HasIndex(e => e.DeactivatedAt);

        // Properties
        entity.Property(e => e.Name)
            .HasMaxLength(128);

        entity.Property(e => e.Slug)
            .HasMaxLength(64);

        entity.Property(e => e.Industry)
            .HasMaxLength(64);

        entity.Property(e => e.Size);

        entity.Property(e => e.LogoUrl)
            .HasMaxLength(512);

        entity.Property(e => e.Status)
            .HasMaxLength(64)
            .HasDefaultValue(AccountStatus.PendingVerification.GetEnumMemberValue())
            .IsRequired();

        entity.Property(e => e.Plan)
            .HasMaxLength(64)
            .HasDefaultValue(AccountPlan.Free.GetEnumMemberValue())
            .IsRequired();

        entity.Property(e => e.TraceId)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(e => e.CreatedBy)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.UpdatedAt)
            .IsRequired();

        entity.Property(e => e.DeletedAt)
            .HasColumnType("timestamp with time zone");

        entity.Property(e => e.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        entity.Property(e => e.DeactivatedAt)
            .HasColumnType("timestamp with time zone");
        
        entity.Property(e => e.DeactivationReason)
            .IsRequired()
            .HasMaxLength(64)
            .HasDefaultValue(AccountBanReason.None.GetEnumMemberValue());
    }
}