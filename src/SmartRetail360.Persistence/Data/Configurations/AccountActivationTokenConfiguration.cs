using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Persistence.Data.Configurations;

public class AccountActivationTokenConfiguration : IEntityTypeConfiguration<AccountActivationToken>
{
    public void Configure(EntityTypeBuilder<AccountActivationToken> entity)
    {
        entity.ToTable("account_activation_tokens");
        
        entity.HasKey(e => e.Id);

        entity.HasIndex(e => e.UserId);
        entity.HasIndex(e => e.TenantId);
        entity.HasIndex(e => e.Token).IsUnique();
        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.ExpiresAt);
        entity.HasIndex(e => e.CreatedAt);
        entity.HasIndex(e => e.Source);
        entity.HasIndex(e => new { e.UserId, e.Status, e.ExpiresAt });
        
        entity.Property(e => e.UserId)
            .IsRequired();
        
        entity.Property(e => e.TenantId)
            .IsRequired();

        entity.Property(e => e.Token)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(e => e.Status)
            .HasMaxLength(32)
            .IsRequired()
            .HasDefaultValue(ActivationTokenStatus.Pending.GetEnumMemberValue());

        entity.Property(e => e.ExpiresAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.TraceId)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(e => e.Source)
            .IsRequired()
            .HasDefaultValue(ActivationSource.None.GetEnumMemberValue());
    }
}