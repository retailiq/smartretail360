using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Infrastructure.Data.Seed.AccessControl;

public static class AbacSeeder
{
    public static IEnumerable<AbacPolicyTemplate> GetPolicyTemplates() => new[]
    {
        new AbacPolicyTemplate
        {
            TemplateName = "TenantScopedView",
            ResourceType = "user",
            Action = "view",
            Environment = "client",
            RuleJson = "{ \"===\": [ { \"var\": \"user.tenant_id\" }, { \"var\": \"resource.tenant_id\" } ] }",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new AbacPolicyTemplate
        {
            TemplateName = "DataScopedView",
            ResourceType = "data", 
            Action = "view",
            Environment = "client",
            RuleJson = """{ "===": [ { "var": "user.tenant_id" }, { "var": "resource.tenant_id" } ] }""",
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }
    };
    
    public static IEnumerable<AbacResourceGroup> GetResourceGroups()
    {
        var now = DateTime.UtcNow;

        return new List<AbacResourceGroup>
        {
            new AbacResourceGroup
            {
                Name = "analytics",
                CreatedAt = now,
                UpdatedAt = now,
                ResourceTypes = new List<AbacResourceTypeGroupMap>
                {
                    new AbacResourceTypeGroupMap
                    {
                        ResourceType = new AbacResourceType
                        {
                            Name = DefaultResourceType.Report.GetEnumMemberValue(),
                            CreatedAt = now,
                            UpdatedAt = now
                        }
                    },
                    new AbacResourceTypeGroupMap
                    {
                        ResourceType = new AbacResourceType
                        {
                            Name = DefaultResourceType.Dashboard.GetEnumMemberValue(),
                            CreatedAt = now,
                            UpdatedAt = now
                        }
                    }
                }
            },
            new AbacResourceGroup
            {
                Name = "data",
                CreatedAt = now,
                UpdatedAt = now,
                ResourceTypes = new List<AbacResourceTypeGroupMap>
                {
                    new AbacResourceTypeGroupMap
                    {
                        ResourceType = new AbacResourceType
                        {
                            Name = DefaultResourceType.Dataset.GetEnumMemberValue(),
                            CreatedAt = now,
                            UpdatedAt = now
                        }
                    },
                    new AbacResourceTypeGroupMap
                    {
                        ResourceType = new AbacResourceType
                        {
                            Name = DefaultResourceType.File.GetEnumMemberValue(),
                            CreatedAt = now,
                            UpdatedAt = now
                        }
                    }
                }
            }
        };
    }
    
    public static IEnumerable<AbacResourceType> GetResourceTypes()
    {
        var now = DateTime.UtcNow;
        return new[]
        {
            new AbacResourceType
                { Name = DefaultResourceType.User.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.Tenant.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.Role.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.Product.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.Order.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.File.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.Dataset.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.Report.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.Dashboard.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.CopilotSession.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.Recommendation.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.ApiKey.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
            new AbacResourceType
                { Name = DefaultResourceType.Webhook.GetEnumMemberValue(), CreatedAt = now, UpdatedAt = now },
        };
    }

    public static IEnumerable<AbacAction> GetActions()
    {
        var now = DateTime.UtcNow;
        return new[]
        {
            new AbacAction
            {
                Name = DefaultActionType.View.GetEnumMemberValue(),
                CreatedAt = now,
                UpdatedAt = now
            },
            new AbacAction
            {
                Name = DefaultActionType.Edit.GetEnumMemberValue(),
                CreatedAt = now,
                UpdatedAt = now
            },
            new AbacAction
            {
                Name = DefaultActionType.Delete.GetEnumMemberValue(),
                CreatedAt = now,
                UpdatedAt = now
            },
            new AbacAction
            {
                Name = DefaultActionType.Create.GetEnumMemberValue(),
                CreatedAt = now,
                UpdatedAt = now
            }
        };
    }

    public static IEnumerable<AbacEnvironment> GetEnvironments()
    {
        var now = DateTime.UtcNow;
        return new[]
        {
            new AbacEnvironment
            {
                Name = DefaultEnvironmentType.Default.GetEnumMemberValue(),
                CreatedAt = now,
                UpdatedAt = now
            },
            new AbacEnvironment
            {
                Name = DefaultEnvironmentType.Client.GetEnumMemberValue(),
                CreatedAt = now,
                UpdatedAt = now
            },
            new AbacEnvironment
            {
                Name = DefaultEnvironmentType.Admin.GetEnumMemberValue(),
                CreatedAt = now,
                UpdatedAt = now
            }
        };
    }
}