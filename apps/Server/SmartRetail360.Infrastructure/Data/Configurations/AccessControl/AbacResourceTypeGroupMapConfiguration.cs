using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities.AccessControl;

namespace SmartRetail360.Infrastructure.Data.Configurations.AccessControl;

public class AbacResourceTypeGroupMapConfiguration : IEntityTypeConfiguration<AbacResourceTypeGroupMap>
{
    public void Configure(EntityTypeBuilder<AbacResourceTypeGroupMap> builder)
    {
        builder.ToTable("abac_resource_type_group_map");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Group)
            .WithMany(g => g.ResourceTypes)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ResourceType)
            .WithMany(r => r.Groups)
            .HasForeignKey(x => x.ResourceTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}