using Microsoft.EntityFrameworkCore;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Persistence.Data;

namespace SmartRetail360.ABAC.Services.AbacPolicyService;

public class AbacPolicySyncService : IAbacPolicySyncService
{
    private readonly AppDbContext _db;

    public AbacPolicySyncService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> SyncPolicyFromTemplateAsync(Guid templateId, string newRuleJson)
    {
        var now = DateTime.UtcNow;

        var targets = await _db.AbacPolicies
            .Where(p => p.TemplateId == templateId && p.AllowTemplateSync && p.RuleJson != newRuleJson)
            .ToListAsync();

        foreach (var policy in targets)
        {
            var backup = new AbacPolicy
            {
                TenantId = policy.TenantId,
                ResourceTypeId = policy.ResourceTypeId,
                ActionId = policy.ActionId,
                AppliesToRole = policy.AppliesToRole,
                RuleJson = policy.RuleJson,
                IsEnabled = policy.IsEnabled,
                TemplateId = policy.TemplateId,
                BasePolicyId = policy.Id,
                VersionNumber = policy.VersionNumber,
                AllowTemplateSync = false,
                CreatedAt = policy.CreatedAt,
                UpdatedAt = policy.UpdatedAt,
                UpdatedBy = policy.UpdatedBy
            };
            _db.AbacPolicies.Add(backup);

            policy.RuleJson = newRuleJson;
            policy.VersionNumber += 1;
            policy.UpdatedAt = now;
        }

        return await _db.SaveChangesAsync();
    }
}