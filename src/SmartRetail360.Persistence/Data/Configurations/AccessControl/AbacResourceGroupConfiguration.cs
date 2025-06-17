using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities.AccessControl;

namespace SmartRetail360.Persistence.Data.Configurations.AccessControl;

public class AbacResourceGroupConfiguration : IEntityTypeConfiguration<AbacResourceGroup>
{
    public void Configure(EntityTypeBuilder<AbacResourceGroup> entity)
    {
        entity.ToTable("abac_resource_groups");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .HasMaxLength(64)
            .IsRequired();
    }
}