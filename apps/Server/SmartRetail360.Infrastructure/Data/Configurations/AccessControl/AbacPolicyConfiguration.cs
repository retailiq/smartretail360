using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities.AccessControl;

namespace SmartRetail360.Infrastructure.Data.Configurations.AccessControl;

public class AbacPolicyConfiguration : IEntityTypeConfiguration<AbacPolicy>
{
    public void Configure(EntityTypeBuilder<AbacPolicy> entity)
    {
        entity.ToTable("abac_policies");

        entity.HasKey(e => e.Id);

        entity.HasIndex(e => new { e.TenantId, e.ResourceTypeId, e.ActionId, e.EnvironmentId, e.Version }).IsUnique();

        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.Version).HasMaxLength(32).IsRequired();
        entity.Property(e => e.RuleJson).IsRequired();
        entity.Property(e => e.IsEnabled).IsRequired();
        entity.Property(e => e.TenantId).HasMaxLength(64).IsRequired();
        entity.Property(e => e.UpdatedBy).HasMaxLength(64).IsRequired();

        entity.HasOne(e => e.ResourceType)
            .WithMany()
            .HasForeignKey(e => e.ResourceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Action)
            .WithMany(a => a.Policies)
            .HasForeignKey(e => e.ActionId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Environment)
            .WithMany(env => env.Policies)
            .HasForeignKey(e => e.EnvironmentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(e => e.BasePolicy)
            .WithMany()
            .HasForeignKey(e => e.BasePolicyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(e => e.Template)
            .WithMany()
            .HasForeignKey(e => e.TemplateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}