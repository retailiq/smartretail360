using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Persistence.Seed.AccessControl;

public static class AbacPolicyActionSeeder
{
    public static IEnumerable<AbacAction> GetActions()
    {
        return
        [
            new AbacAction
            {
                Name = DefaultActionType.View.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.Edit.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.Delete.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.Create.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.Send.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.Leave.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.Remove.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.Suspend.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.Unsuspend.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.Invite.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.Audit.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.TenantDeactivate.GetEnumMemberValue()
            },
            new AbacAction
            {
                Name = DefaultActionType.TenantReactivate.GetEnumMemberValue()
            }
        ];
    }
}