using Microsoft.EntityFrameworkCore;
using SmartRetail360.Persistence.Data;

namespace SmartRetail360.Persistence.Seed;

public static class SystemRoleDbRunner
{
    public static async Task RunAsync(AppDbContext db)
    {
        // Ensure a database is created
        await db.Database.MigrateAsync();

        // Write into a database if it is empty
        if (!await db.Roles.AnyAsync())
        {
            var systemRoles = RoleSeeder.GetSystemRoles();
            db.Roles.AddRange(systemRoles);
            await db.SaveChangesAsync();
        }
        else
        {
            Console.WriteLine("[Seed] System roles already exist in DB. Skipped.");
        }
    }
}