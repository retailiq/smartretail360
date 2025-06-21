using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Persistence.Data.Configurations;

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
            .HasMaxLength(255);

        entity.Property(e => e.PhoneNumber)
            .HasMaxLength(32);

        entity.Property(e => e.CountryCode)
            .HasMaxLength(5);

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

        entity.Property(e => e.IsSystemAccount)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.UpdatedAt)
            .IsRequired();

        entity.Property(e => e.LastEmailSentAt)
            .HasColumnType(GeneralConstants.TimestampWithTimeZone);

        entity.Property(e => e.LastLoginAt)
            .HasColumnType(GeneralConstants.TimestampWithTimeZone);

        entity.Property(e => e.DeactivatedAt)
            .HasColumnType(GeneralConstants.TimestampWithTimeZone);

        entity.Property(e => e.DeletedAt)
            .HasColumnType(GeneralConstants.TimestampWithTimeZone);

        entity.HasMany(u => u.OAuthAccounts)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .HasConstraintName("FK_oauth_accounts_users_UserId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}