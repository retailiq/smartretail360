using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Persistence.Seed.AccessControl;

public static class AbacPolicyEnvironmentSeeder
{
    public static IEnumerable<AbacEnvironment> GetEnvironments()
    {
        return
        [
            new AbacEnvironment
            {
                Name = DefaultEnvironmentType.Default.GetEnumMemberValue(),
            },
            new AbacEnvironment
            {
                Name = DefaultEnvironmentType.Client.GetEnumMemberValue(),
            },
            new AbacEnvironment
            {
                Name = DefaultEnvironmentType.Admin.GetEnumMemberValue(),
            },
            new AbacEnvironment
            {
                Name = DefaultEnvironmentType.System.GetEnumMemberValue(),
            }
        ];
    }
}