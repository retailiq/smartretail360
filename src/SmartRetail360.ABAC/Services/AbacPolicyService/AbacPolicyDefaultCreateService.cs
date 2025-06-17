using Microsoft.EntityFrameworkCore;
using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Execution;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.ABAC.Services.AbacPolicyService;

public class AbacPolicyDefaultCreateService : IAbacPolicyDefaultCreateService
{
    private readonly AppDbContext _db;
    private readonly IPolicyRepo _repo;
    private readonly ISafeExecutor _safeExecutor;

    public AbacPolicyDefaultCreateService(
        AppDbContext db,
        IPolicyRepo repo,
        ISafeExecutor safeExecutor)
    {
        _db = db;
        _repo = repo;
        _safeExecutor = safeExecutor;
    }

    public async Task CreateDefaultPoliciesForTenantAsync(Guid tenantId, bool enableImmediately = false)
    {
        var resourceMap = await _repo.GetAllResourceTypeMapAsync();
        var actionMap = await _repo.GetAllActionMapAsync();
        var templates = await _repo.GetPolicyTemplatesMapAsync();
        var resourceGroupMap = await _repo.GetAllResourceGroupMapAsync();
        
        foreach (var template in templates.Where(t => t.IsEnabled))
        {
            var expandedResources = new List<string>();

            if (resourceGroupMap.TryGetValue(template.ResourceType, out var groupResources))
            {
                expandedResources.AddRange(groupResources);
            }
            else if (resourceMap.ContainsKey(template.ResourceType))
            {
                expandedResources.Add(template.ResourceType);
            }

            foreach (var res in expandedResources)
            {
                if (!resourceMap.TryGetValue(res, out var resourceId)) continue;
                if (!actionMap.TryGetValue(template.Action.ToLowerInvariant(), out var actionId)) continue;

                bool exists = await _db.AbacPolicies.AnyAsync(p =>
                    p.TenantId == tenantId &&
                    p.ResourceTypeId == resourceId &&
                    p.ActionId == actionId &&
                    p.VersionNumber == 1);

                if (exists) continue;

                _db.AbacPolicies.Add(new AbacPolicy
                {
                    TenantId = tenantId,
                    ResourceTypeId = resourceId,
                    ActionId = actionId,
                    RuleJson = template.RuleJson,
                    TemplateId = template.Id,
                    IsEnabled = enableImmediately,
                    AllowTemplateSync = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    VersionNumber = 1
                });
            }
        }

        await _safeExecutor.ExecuteAsync(
            async () => { await _db.SaveChangesAsync(); },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );
    }
}