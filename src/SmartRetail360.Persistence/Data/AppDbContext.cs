using Microsoft.EntityFrameworkCore;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Domain.Entities.AccessControl;

namespace SmartRetail360.Persistence.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<AccountActivationToken> AccountActivationTokens => Set<AccountActivationToken>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OAuthAccount> OAuthAccounts => Set<OAuthAccount>();
    public DbSet<AbacPolicy> AbacPolicies => Set<AbacPolicy>();
    public DbSet<AbacResourceType> AbacResourceTypes => Set<AbacResourceType>();
    public DbSet<AbacAction> AbacActions => Set<AbacAction>();
    public DbSet<AbacEnvironment> AbacEnvironments => Set<AbacEnvironment>();
    public DbSet<AbacPolicyTemplate> AbacPolicyTemplates => Set<AbacPolicyTemplate>();
    public DbSet<AbacResourceGroup> AbacResourceGroups => Set<AbacResourceGroup>();
    public DbSet<AbacResourceTypeGroupMap> AbacResourceTypeGroupMaps => Set<AbacResourceTypeGroupMap>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from the assembly automatically
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global filters
        modelBuilder.Entity<Role>().HasQueryFilter(r => r.IsSystemRole);
        modelBuilder.Entity<AbacPolicy>().Ignore(x => x.Version);
    }
}