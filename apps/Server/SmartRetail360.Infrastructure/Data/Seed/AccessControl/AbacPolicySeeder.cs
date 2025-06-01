using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Infrastructure.Data.Seed.AccessControl;

public static class AbacPolicySeeder
{
    public static IEnumerable<AbacPolicy> GetPolicies(
        Dictionary<string, Guid> resourceMap,
        Dictionary<string, Guid> actionMap,
        Dictionary<string, Guid> envMap)
    {
        return new[]
        {
            // User:View
            new AbacPolicy
            {
                ResourceTypeId = resourceMap[DefaultResourceType.User.GetEnumMemberValue()],
                ActionId = actionMap[DefaultActionType.View.GetEnumMemberValue()],
                EnvironmentId = envMap[DefaultEnvironmentType.Default.GetEnumMemberValue()],
                RuleJson = $$"""
                             {
                               "and": [
                                 { "==": [ { "var": "user.tenant_id" }, { "var": "resource.tenant_id" } ] },
                                 { "in": [ { "var": "resource.role_name" }, [ "{{SystemRoleType.Admin.GetEnumMemberValue()}}", "{{SystemRoleType.Owner.GetEnumMemberValue()}}" ] ] }
                               ]
                             }
                             """
            },
            // ✅ User:Edit — Admin/Owner can edit their own user profile
            new AbacPolicy
            {
                ResourceTypeId = resourceMap[DefaultResourceType.User.GetEnumMemberValue()],
                ActionId = actionMap[DefaultActionType.Edit.GetEnumMemberValue()],
                EnvironmentId = envMap[DefaultEnvironmentType.Default.GetEnumMemberValue()],
                RuleJson = $$"""
                             {
                               "in": [
                                 { "var": "resource.role_name" },
                                 [ "{{SystemRoleType.Admin.GetEnumMemberValue()}}", "{{SystemRoleType.Owner.GetEnumMemberValue()}}" ]
                               ]
                             }
                             """
            }
        };
    }
}