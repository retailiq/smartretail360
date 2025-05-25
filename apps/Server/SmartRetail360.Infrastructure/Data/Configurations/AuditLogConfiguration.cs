using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRetail360.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata;

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

        auditLog.Property(x => x.Level)
            .HasConversion<string>()
            .IsRequired();
        
        auditLog.Property(x => x.SourceModule)
            .IsRequired(false);
        
        auditLog.Property(x => x.LogId)
            .IsRequired();      

        auditLog.Property(x => x.DetailsJson)
            .HasColumnName("Details")
            .HasColumnType("jsonb")
            .IsRequired(false);
        
        foreach (var property in auditLog.Metadata.GetProperties())
        {
            if (!property.IsPrimaryKey())
            {
                property.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            }
        }
    }
}