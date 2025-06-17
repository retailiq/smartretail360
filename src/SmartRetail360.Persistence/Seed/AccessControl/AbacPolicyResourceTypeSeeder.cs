using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Persistence.Seed.AccessControl;

public static class AbacPolicyResourceTypeSeeder
{
    public static IEnumerable<AbacResourceType> GetResourceTypes()
    {
        return
        [
            new AbacResourceType
                { Name = DefaultResourceType.User.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.Tenant.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.Role.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.Product.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.Order.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.File.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.Dataset.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.Report.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.Dashboard.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.CopilotSession.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.Recommendation.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.ApiKey.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.Webhook.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.Notification.GetEnumMemberValue() },
            new AbacResourceType
                { Name = DefaultResourceType.AbacPolicy.GetEnumMemberValue() }
        ];
    }
}