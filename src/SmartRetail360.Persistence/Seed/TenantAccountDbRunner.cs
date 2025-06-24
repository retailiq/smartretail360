using Microsoft.EntityFrameworkCore;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Persistence.Seed;

public static class TenantAccountDbRunner
{
    private static readonly Guid SystemTenantId = Guid.Parse("6e13c1c0-91c7-4e67-94c6-6f6c05d3ef72");
    private const string Name = "SmartRetail360";
    private const string Email = "sr360@danieltate.net";
    private const string Password = "Daniel+123";

    public static async Task RunAsync(AppDbContext db)
    {
        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == Email.ToLower());
        if (existingUser != null)
        {
            Console.WriteLine($"[Seed] Skipped: User {Email} already exists.");
            return;
        }

        var traceId = Guid.NewGuid().ToString("N");
        var passwordHash = PasswordHelper.HashPassword(Password);

        var ownerRoleName = SystemRoleType.Owner.GetEnumMemberValue();
        var ownerRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == ownerRoleName && r.IsSystemRole);

        if (ownerRole == null)
        {
            Console.WriteLine("[Seed][Error] Owner role not found. Please ensure system roles are seeded first.");
            return;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = Email.ToLowerInvariant(),
            Name = Name,
            PasswordHash = passwordHash,
            TraceId = traceId,
            IsEmailVerified = true,
            LocaleEnum = LocaleType.En,
            IsFirstLogin = false,
            StatusEnum = AccountStatus.Active,
            IsActive = true,
            IsSystemAccount = true,
        };

        var tenant = new Tenant
        {
            Id = SystemTenantId,
            TraceId = traceId,
            CreatedBy = user.Id,
            Slug = SlugGenerator.GenerateSlug(Name),
            Name = Name,
            StatusEnum = AccountStatus.Active,
            PlanEnum = AccountPlan.System,
            IsActive = true,
        };

        var tenantUser = new TenantUser
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TenantId = tenant.Id,
            RoleId = ownerRole.Id,
            TraceId = traceId,
            CreatedBy = user.Id,
            IsDefault = true,
            IsActive = true,
        };

        try
        {
            db.Users.Add(user);
            db.Tenants.Add(tenant);
            db.TenantUsers.Add(tenantUser);

            await db.SaveChangesAsync();
            Console.WriteLine("[Seed] System Tenant + User + TenantUser created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Seed][Error]  Failed to create System Tenant + User + TenantUser: {ex.Message}");
            Console.WriteLine($"[Seed][InnerException] {ex.InnerException?.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }
}