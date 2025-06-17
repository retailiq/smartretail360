using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Persistence.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> entity)
    {
        entity.ToTable("roles");

        // Primary key
        entity.HasKey(e => e.Id);

        // Indexes
        entity.HasIndex(e => e.Name).IsUnique();
        entity.HasIndex(e => e.IsSystemRole);
        entity.HasIndex(e => e.CreatedAt);

        // Properties
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(32)
            .HasDefaultValue(SystemRoleType.Member.GetEnumMemberValue());
        
        entity.Property(e => e.IsSystemRole)
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}