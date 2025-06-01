using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities.AccessControl;

namespace SmartRetail360.Infrastructure.Data.Configurations.AccessControl;

public class AbacPolicyTemplateConfiguration : IEntityTypeConfiguration<AbacPolicyTemplate>
{
    public void Configure(EntityTypeBuilder<AbacPolicyTemplate> entity)
    {
        entity.ToTable("abac_policy_templates");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.TemplateName)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(e => e.ResourceType)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(e => e.Action)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(e => e.Environment)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(e => e.RuleJson).IsRequired();

        entity.Property(e => e.IsEnabled).IsRequired();

        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
    }
}