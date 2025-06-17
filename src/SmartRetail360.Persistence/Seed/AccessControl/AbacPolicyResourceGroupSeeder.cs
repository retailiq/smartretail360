using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Persistence.Seed.AccessControl;

public static class AbacPolicyResourceGroupSeeder
{
    public static IEnumerable<AbacResourceGroup> GetResourceGroups()
    {
        return
        [
            new AbacResourceGroup
            {
                Name = "analytics",
                ResourceTypes = new List<AbacResourceTypeGroupMap>
                {
                    new AbacResourceTypeGroupMap
                    {
                        ResourceType = new AbacResourceType
                        {
                            Name = DefaultResourceType.Report.GetEnumMemberValue()
                        }
                    },
                    new AbacResourceTypeGroupMap
                    {
                        ResourceType = new AbacResourceType
                        {
                            Name = DefaultResourceType.Dashboard.GetEnumMemberValue()
                        }
                    }
                }
            },
            new AbacResourceGroup
            {
                Name = "data",
                ResourceTypes = new List<AbacResourceTypeGroupMap>
                {
                    new AbacResourceTypeGroupMap
                    {
                        ResourceType = new AbacResourceType
                        {
                            Name = DefaultResourceType.Dataset.GetEnumMemberValue()
                        }
                    },
                    new AbacResourceTypeGroupMap
                    {
                        ResourceType = new AbacResourceType
                        {
                            Name = DefaultResourceType.File.GetEnumMemberValue()
                        }
                    }
                }
            }
        ];
    }
}