using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> entity)
    {
        entity.ToTable("tenants");

        entity.HasKey(e => e.Id);
        entity.HasIndex(t => t.DeletedAt);
        entity.HasIndex(t => t.Slug).IsUnique();
        entity.HasIndex(a => a.AdminEmail).IsUnique();

        entity.Property(e => e.Name).HasMaxLength(128);
        entity.Property(e => e.Slug).HasMaxLength(64).IsRequired();
        entity.Property(e => e.AdminEmail).HasMaxLength(128).IsRequired();
        entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
        entity.Property(e => e.Industry).HasMaxLength(64);
        entity.Property(e => e.PhoneNumber).HasMaxLength(32);
        entity.Property(e => e.Status)
            .HasDefaultValue(StringCaseConverter.ToSnakeCase(nameof(TenantStatus.PendingVerification)));
        entity.Property(e => e.Plan)
            .HasDefaultValue(StringCaseConverter.ToSnakeCase(nameof(AccountPlan.Free)));
        entity.Property(e => e.IsEmailVerified).IsRequired().HasDefaultValue(false);
        entity.Property(e => e.IsFirstLogin).IsRequired().HasDefaultValue(true);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.LastEmailSentAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
    }
}