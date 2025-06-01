using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Infrastructure.Data.Configurations.AccessControl;

public class AbacResourceTypeConfiguration : IEntityTypeConfiguration<AbacResourceType>
{
    public void Configure(EntityTypeBuilder<AbacResourceType> entity)
    {
        entity.ToTable("abac_resource_types");

        entity.HasKey(e => e.Id);

        entity.HasIndex(e => e.Name).IsUnique();

        entity.Property(e => e.Name)
            .HasMaxLength(64)
            .HasDefaultValue(DefaultResourceType.None.GetEnumMemberValue())
            .IsRequired();

        entity.Property(e => e.CreatedAt)
            .IsRequired();

        entity.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}