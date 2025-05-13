// using Microsoft.EntityFrameworkCore;
// using SmartRetail360.Domain.Entities;
// using SmartRetail360.Shared.Enums;
// using SmartRetail360.Shared.Utils;
//
// namespace SmartRetail360.Infrastructure.Data;
//
// public class AppDbContext : DbContext
// {
//     public AppDbContext(DbContextOptions<AppDbContext> options)
//         : base(options)
//     {
//     }
//     
//     public DbSet<Tenant> Tenants { get; set; } = null!;
//
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         base.OnModelCreating(modelBuilder);
//         
//         modelBuilder.Entity<Tenant>().HasKey(e => e.Id);
//         modelBuilder.Entity<Tenant>().HasIndex(t => t.DeletedAt);
//         modelBuilder.Entity<Tenant>().HasIndex(t => t.Slug).IsUnique();
//         modelBuilder.Entity<Tenant>().HasIndex(a => a.AdminEmail).IsUnique();
//         
//         modelBuilder.Entity<Tenant>(entity =>
//         {
//             entity.Property(e => e.Name).HasMaxLength(128);
//             entity.Property(e => e.Slug).HasMaxLength(64).IsRequired();
//             entity.Property(e => e.AdminEmail).HasMaxLength(128).IsRequired();
//             entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
//             entity.Property(e => e.Industry).HasMaxLength(64);
//             entity.Property(e => e.PhoneNumber).HasMaxLength(32);
//             entity.Property(e => e.Status)
//                 .HasDefaultValue(StringCaseConverter.ToSnakeCase(nameof(TenantStatus.PendingVerification)));
//             entity.Property(e => e.Plan).HasDefaultValue(StringCaseConverter.ToSnakeCase(nameof(AccountPlan.Free)));
//             entity.Property(e => e.IsEmailVerified).IsRequired().HasDefaultValue(false);
//             entity.Property(e => e.IsFirstLogin).IsRequired().HasDefaultValue(true);
//             entity.Property(e => e.CreatedAt).IsRequired();
//             entity.Property(e => e.UpdatedAt).IsRequired();
//             entity.Property(e => e.LastEmailSentAt)
//                 .HasColumnType("timestamp with time zone")
//                 .IsRequired(false);
//         });
//     }
// }

using Microsoft.EntityFrameworkCore;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Data.Configurations;

namespace SmartRetail360.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // modelBuilder.ApplyConfiguration(new TenantConfiguration());

        // 如果有多个配置类，可启用自动扫描：
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}