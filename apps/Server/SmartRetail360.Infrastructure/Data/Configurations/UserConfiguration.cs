using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("users");

        // Primary key
        entity.HasKey(e => e.Id);

        // Indexes
        entity.HasIndex(e => e.Email).IsUnique();
        entity.HasIndex(e => e.DeletedAt);
        entity.HasIndex(e => e.TraceId);
        entity.HasIndex(e => e.LastLoginAt);
        entity.HasIndex(e => e.IsActive);
        entity.HasIndex(e => new { e.IsEmailVerified, e.Status });
        entity.HasIndex(e => e.DeactivatedAt);
        entity.HasIndex(e => e.CreatedAt);

        // Property configurations
        entity.Property(e => e.Name)
            .HasMaxLength(128);

        entity.Property(e => e.Email)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(e => e.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(e => e.PhoneNumber)
            .HasMaxLength(32);

        entity.Property(e => e.AvatarUrl)
            .HasMaxLength(512);

        entity.Property(e => e.Locale)
            .IsRequired()
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValue(LocaleType.En.GetEnumMemberValue());

        entity.Property(e => e.Status)
            .HasMaxLength(64)
            .IsRequired()
            .HasDefaultValue(AccountStatus.PendingVerification.GetEnumMemberValue());

        entity.Property(e => e.IsEmailVerified)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.IsFirstLogin)
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(e => e.TraceId)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(e => e.DeactivationReason)
            .IsRequired()
            .HasMaxLength(64)
            .HasDefaultValue(AccountBanReason.None.GetEnumMemberValue());

        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.UpdatedAt)
            .IsRequired();

        entity.Property(e => e.LastEmailSentAt)
            .HasColumnType("timestamp with time zone");

        entity.Property(e => e.LastLoginAt)
            .HasColumnType("timestamp with time zone");

        entity.Property(e => e.DeactivatedAt)
            .HasColumnType("timestamp with time zone");

        entity.Property(e => e.DeletedAt)
            .HasColumnType("timestamp with time zone");
    }
}