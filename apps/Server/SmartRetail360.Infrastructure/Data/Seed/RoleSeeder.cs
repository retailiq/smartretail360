using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Data.Seed;

public static class RoleSeeder
{
    public static IEnumerable<Role> GetSystemRoles()
    {
        return new List<Role>
        {
            new Role { Name = StringCaseConverter.ToSnakeCase(nameof(SystemRoleType.Owner)),  IsSystemRole = true },
            new Role { Name = StringCaseConverter.ToSnakeCase(nameof(SystemRoleType.Admin)),  IsSystemRole = true },
            new Role { Name = StringCaseConverter.ToSnakeCase(nameof(SystemRoleType.Analyst)),  IsSystemRole = true },
            new Role { Name = StringCaseConverter.ToSnakeCase(nameof(SystemRoleType.Member)),  IsSystemRole = true },
            new Role { Name = StringCaseConverter.ToSnakeCase(nameof(SystemRoleType.Guest)), IsSystemRole = true },
        };
    }
}
