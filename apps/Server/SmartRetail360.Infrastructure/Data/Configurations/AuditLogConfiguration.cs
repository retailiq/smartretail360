using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;

namespace SmartRetail360.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> auditLog)
    {
        auditLog.ToTable("audit_logs");

        auditLog.HasKey(x => x.Id);

        auditLog.Property(x => x.Action)
            .IsRequired();

        auditLog.Property(x => x.IsSuccess)
            .IsRequired();

        auditLog.Property(x => x.EvaluatedAt)
            .IsRequired();

        auditLog.Property(x => x.TraceId)
            .IsRequired();

        auditLog.Property(x => x.DetailsJson)
            .HasColumnName("Details")
            .HasColumnType("jsonb")
            .IsRequired(false);
    }
}