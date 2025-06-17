using Microsoft.EntityFrameworkCore;
using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Persistence;
using SmartRetail360.Persistence.Data;

namespace SmartRetail360.ABAC.Services.AbacPolicyService;

public class AbacPolicyApplyTemplateService : IAbacPolicyApplyTemplateService
{
    private readonly AppDbContext _db;
    private readonly IPolicyRepo _repo;

    public AbacPolicyApplyTemplateService(AppDbContext db, IPolicyRepo repo)
    {
        _db = db;
        _repo = repo;
    }

    public async Task<int> ApplyPolicyTemplatesToTenantAsync(Guid tenantId)
    {
        var templates = await _db.AbacPolicyTemplates.AsNoTracking()
            .Where(t => t.IsEnabled)
            .ToListAsync();
        
        var resourceMap = await _repo.GetAllResourceTypeMapAsync();
        var actionMap = await _repo.GetAllActionMapAsync();

        var now = DateTime.UtcNow;

        var newPolicies = templates.Select(t =>
        {
            var resourceId = resourceMap.GetValueOrDefault(t.ResourceType);
            var actionId = actionMap.GetValueOrDefault(t.Action);

            if (resourceId == Guid.Empty || actionId == Guid.Empty)
                return null;

            return new AbacPolicy
            {
                TenantId = tenantId,
                ResourceTypeId = resourceId,
                ActionId = actionId,
                RuleJson = t.RuleJson,
                IsEnabled = t.IsEnabled,
                CreatedAt = now,
                UpdatedAt = now,
                UpdatedBy = Guid.Empty
            };
        }).Where(p => p != null).ToList()!;

        await _db.AbacPolicies.AddRangeAsync(newPolicies);
        await _db.SaveChangesAsync();

        return newPolicies.Count;
    }
}
