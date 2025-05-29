using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Infrastructure.Data.Configurations;

public class OAuthAccountConfiguration : IEntityTypeConfiguration<OAuthAccount>
{
    public void Configure(EntityTypeBuilder<OAuthAccount> entity)
    {
        entity.ToTable("oauth_accounts");

        entity.HasKey(e => e.Id);

        entity.HasIndex(e => e.Email);
        entity.HasIndex(e => e.ProviderUserId);
        entity.HasIndex(e => e.Provider);
        entity.HasIndex(e => e.UserId);
        entity.HasIndex(e => e.TraceId);
        entity.HasIndex(e => new { e.Email, e.Provider });

        entity.Property(e => e.Name)
            .HasMaxLength(128);

        entity.Property(e => e.Email)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(e => e.UserId)
            .IsRequired();

        entity.Property(e => e.Provider)
            .IsRequired()
            .HasMaxLength(128)
            .HasDefaultValue(OAuthProvider.None.GetEnumMemberValue());

        entity.Property(e => e.ProviderUserId)
            .IsRequired()
            .HasMaxLength(128);

        entity.Property(e => e.AvatarUrl)
            .HasMaxLength(512);

        entity.Property(e => e.TraceId)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.UpdatedAt)
            .IsRequired();
        
        entity.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(64)
            .HasDefaultValue(AccountStatus.Active.GetEnumMemberValue());
        
        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        
        entity.Property(e => e.DeactivatedAt)
            .HasColumnType("timestamp with time zone");
        
        entity.Property(e => e.DeactivationReason)
            .IsRequired()
            .HasMaxLength(64)
            .HasDefaultValue(AccountBanReason.None.GetEnumMemberValue());
        
        entity.HasOne(e => e.User)
            .WithMany(u => u.OAuthAccounts)
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_oauth_accounts_users_UserId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}