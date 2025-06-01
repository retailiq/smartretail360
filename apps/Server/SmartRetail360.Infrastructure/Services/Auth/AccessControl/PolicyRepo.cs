using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Auth.AccessControl;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Infrastructure.Data;
using System.Text.Json;

namespace SmartRetail360.Infrastructure.Services.Auth.AccessControl;

public class PolicyRepo : IPolicyRepo
{
    private readonly AppDbContext _db;

    public PolicyRepo(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<AbacPolicyTemplate>> GetPolicyTemplatesAsync()
    {
        return await _db.AbacPolicyTemplates.AsNoTracking()
            .Where(t => t.IsEnabled)
            .ToListAsync();
    }

    public async Task<List<string>> GetGroupResourcesAsync(string groupName)
    {
        return await _db.AbacResourceGroups
            .AsNoTracking()
            .Where(g => g.Name == groupName)
            .SelectMany(g => g.ResourceTypes.Select(rt => rt.ResourceType.Name))
            .ToListAsync();
    }

    public async Task<string?> GetPolicyJsonAsync(Guid tenantId, string resourceType, string action, string environmentName)
    {
        var policy = await _db.AbacPolicies
            .Include(p => p.ResourceType)
            .Include(p => p.Action)
            .Include(p => p.Environment)
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.ResourceType.Name == resourceType &&
                p.Action.Name == action &&
                p.TenantId == tenantId &&
                p.Environment.Name == environmentName);

        Console.WriteLine($"[ABAC] DB Lookup Result for key ({tenantId}, {resourceType}, {action}, {environmentName}): {(policy == null ? "Not Found" : "Found")}");
        return policy?.RuleJson;
    }

    public async Task<Dictionary<string, Guid>> GetAllEnvironmentMapAsync()
    {
        var envs = await _db.AbacEnvironments.AsNoTracking().ToListAsync();
        return envs.ToDictionary(e => e.Name, e => e.Id);
    }

    public async Task<Dictionary<string, Guid>> GetAllResourceTypeMapAsync()
    {
        var types = await _db.AbacResourceTypes.AsNoTracking().ToListAsync();
        return types.ToDictionary(t => t.Name, t => t.Id);
    }

    public async Task<Dictionary<string, List<string>>> GetAllResourceGroupMapAsync()
    {
        return await _db.AbacResourceGroups
            .Include(g => g.ResourceTypes)
            .ThenInclude(m => m.ResourceType)
            .AsNoTracking()
            .ToDictionaryAsync(
                g => g.Name,
                g => g.ResourceTypes.Select(rt => rt.ResourceType.Name).ToList()
            );
    }

    public async Task<Dictionary<string, Guid>> GetAllActionMapAsync()
    {
        var actions = await _db.AbacActions.AsNoTracking().ToListAsync();
        return actions.ToDictionary(a => a.Name.ToLowerInvariant(), a => a.Id);
    }
}