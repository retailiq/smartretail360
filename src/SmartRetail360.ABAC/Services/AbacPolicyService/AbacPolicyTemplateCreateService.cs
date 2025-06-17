using Microsoft.EntityFrameworkCore;
using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Persistence;
using SmartRetail360.Persistence.Data;

namespace SmartRetail360.ABAC.Services.AbacPolicyService;

public class AbacPolicyTemplateCreateService : IAbacPolicyTemplateCreateService
{
    private readonly AppDbContext _db;
    private readonly IPolicyRepo _repo;

    public AbacPolicyTemplateCreateService(AppDbContext db, IPolicyRepo repo)
    {
        _db = db;
        _repo = repo;
    }

    public async Task CreatePoliciesFromTemplateAsync(Guid tenantId, string templateName)
    {
        var templates = await _db.AbacPolicyTemplates
            .Where(t => t.TemplateName == templateName && t.IsEnabled)
            .ToListAsync();

        if (!templates.Any()) return;

        var resourceMap = await _repo.GetAllResourceTypeMapAsync();
        var actionMap = await _repo.GetAllActionMapAsync();

        var newPolicies = new List<AbacPolicy>();

        foreach (var tpl in templates)
        {
            if (!resourceMap.TryGetValue(tpl.ResourceType, out var resourceId) ||
                !actionMap.TryGetValue(tpl.Action, out var actionId))
                continue;

            var exists = await _db.AbacPolicies.AnyAsync(p =>
                p.TenantId == tenantId &&
                p.ResourceTypeId == resourceId &&
                p.ActionId == actionId);

            if (exists) continue;

            newPolicies.Add(new AbacPolicy
            {
                TenantId = tenantId,
                ResourceTypeId = resourceId,
                ActionId = actionId,
                RuleJson = tpl.RuleJson,
                IsEnabled = true,
                VersionNumber = 1
            });
        }

        if (newPolicies.Any())
        {
            await _db.AbacPolicies.AddRangeAsync(newPolicies);
            await _db.SaveChangesAsync();
        }
    }
}