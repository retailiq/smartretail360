using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Shared.Redis;
using StackExchange.Redis;
using SmartRetail360.Infrastructure.Data.Seed.AccessControl;
using SmartRetail360.Domain.Entities.AccessControl;

namespace SmartRetail360.Infrastructure.Data.Seed.AccessControl;

public static class AbacSeedRunner
{
    public static async Task RunAsync(AppDbContext db, IConnectionMultiplexer redis)
    {
        var redisDb = redis.GetDatabase();
        var server = redis.GetServer(redis.GetEndPoints().First());

        async Task DeleteKeysAsync(string pattern)
        {
            var keys = server.Keys(pattern: pattern);
            foreach (var key in keys)
            {
                await redisDb.KeyDeleteAsync(key);
            }
        }

        await DeleteKeysAsync("abac:resource-type:*");
        await DeleteKeysAsync("abac:action:*");
        await DeleteKeysAsync("abac:environment:*");
        await DeleteKeysAsync("abac:policy:*");
        await DeleteKeysAsync("abac:resource-group:*");
        await DeleteKeysAsync("abac:policy-template:*");

        // ---------- 1. ResourceTypes ----------
        var seedResources = AbacSeeder.GetResourceTypes().ToList();
        var existingResources = await db.AbacResourceTypes.ToListAsync();

        db.AbacResourceTypes.RemoveRange(existingResources
            .Where(er => seedResources.All(sr => sr.Name != er.Name)));

        await db.AbacResourceTypes.AddRangeAsync(seedResources
            .Where(sr => existingResources.All(er => er.Name != sr.Name)));

        await db.SaveChangesAsync();

        var dbResources = await db.AbacResourceTypes.AsNoTracking().ToListAsync();
        foreach (var r in dbResources)
        {
            await redisDb.StringSetAsync(
                RedisKeys.AbacResourceType(r.Name),
                JsonSerializer.Serialize(r, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        // ---------- 2. Actions ----------
        var seedActions = AbacSeeder.GetActions().ToList();
        var existingActions = await db.AbacActions.ToListAsync();

        db.AbacActions.RemoveRange(existingActions
            .Where(ea => seedActions.All(sa => sa.Name != ea.Name)));

        await db.AbacActions.AddRangeAsync(seedActions
            .Where(sa => existingActions.All(ea => ea.Name != sa.Name)));

        await db.SaveChangesAsync();

        var dbActions = await db.AbacActions.AsNoTracking().ToListAsync();
        foreach (var a in dbActions)
        {
            await redisDb.StringSetAsync(
                RedisKeys.AbacAction(a.Name),
                JsonSerializer.Serialize(a, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        // ---------- 3. Environments ----------
        var seedEnvs = AbacSeeder.GetEnvironments().ToList();
        var existingEnvs = await db.AbacEnvironments.ToListAsync();

        db.AbacEnvironments.RemoveRange(existingEnvs
            .Where(ee => seedEnvs.All(se => se.Name != ee.Name)));

        await db.AbacEnvironments.AddRangeAsync(seedEnvs
            .Where(se => existingEnvs.All(ee => ee.Name != se.Name)));

        await db.SaveChangesAsync();

        var dbEnvs = await db.AbacEnvironments.AsNoTracking().ToListAsync();
        foreach (var e in dbEnvs)
        {
            await redisDb.StringSetAsync(
                RedisKeys.AbacEnvironment(e.Name),
                JsonSerializer.Serialize(e, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        // ---------- 4. Policies ----------
        var dbPolicies = await db.AbacPolicies.AsNoTracking().ToListAsync();
        foreach (var p in dbPolicies)
        {
            var resName = dbResources.First(r => r.Id == p.ResourceTypeId).Name;
            var actName = dbActions.First(a => a.Id == p.ActionId).Name;
            var envName = dbEnvs.First(e => e.Id == p.EnvironmentId).Name;

            await redisDb.StringSetAsync(
                RedisKeys.AbacPolicy(p.TenantId, resName, actName, envName),
                JsonSerializer.Serialize(p, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        // ---------- 5. Resource Groups ----------
        var seedGroups = AbacSeeder.GetResourceGroups().ToList();
        var existingGroups = await db.AbacResourceGroups
            .Include(g => g.ResourceTypes)
            .ThenInclude(gm => gm.ResourceType)
            .ToListAsync();

        db.AbacResourceGroups.RemoveRange(existingGroups);
        await db.SaveChangesAsync();

        var dbResourceMap = await db.AbacResourceTypes.ToDictionaryAsync(r => r.Name);

        foreach (var group in seedGroups)
        {
            var toReplace = new List<(AbacResourceTypeGroupMap GroupMap, AbacResourceType Existing)>();

            foreach (var map in group.ResourceTypes)
            {
                if (dbResourceMap.TryGetValue(map.ResourceType.Name, out var existing))
                {
                    toReplace.Add((map, existing));
                }
            }

            foreach (var (map, existing) in toReplace)
            {
                group.ResourceTypes.Remove(map);
                existing.Group = group;
            }
        }

        await db.AbacResourceGroups.AddRangeAsync(seedGroups);
        await db.SaveChangesAsync();

        db.AbacResourceTypeGroupMaps.RemoveRange(db.AbacResourceTypeGroupMaps);
        await db.SaveChangesAsync();

        var resourceGroups = await db.AbacResourceGroups.AsNoTracking().ToListAsync();
        var resourceMap = await db.AbacResourceTypes.AsNoTracking().ToDictionaryAsync(r => r.Name);

        Guid GetGroupId(string groupName) => resourceGroups.First(g => g.Name == groupName).Id;
        Guid GetResourceId(string resourceName) => resourceMap[resourceName].Id;

        var groupMap = new List<AbacResourceTypeGroupMap>
        {
            new() { GroupId = GetGroupId("data"), ResourceTypeId = GetResourceId("product") }
        };

        await db.AbacResourceTypeGroupMaps.AddRangeAsync(groupMap);
        await db.SaveChangesAsync();

        var dbGroups = await db.AbacResourceGroups
            .Include(g => g.ResourceTypes)
            .ThenInclude(m => m.ResourceType)
            .AsNoTracking()
            .ToListAsync();

        foreach (var g in dbGroups)
        {
            await redisDb.StringSetAsync(
                RedisKeys.AbacResourceGroup(g.Name),
                JsonSerializer.Serialize(new
                {
                    g.Id,
                    g.Name,
                    ResourceTypes = g.ResourceTypes.Select(rt => rt.ResourceType.Name).ToList()
                }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }

        // ---------- 6. Policy Templates ----------
        var templates = AbacSeeder.GetPolicyTemplates().ToList();
        var existingTemplates = await db.AbacPolicyTemplates.ToListAsync();

        db.AbacPolicyTemplates.RemoveRange(existingTemplates
            .Where(et => templates.All(nt => nt.TemplateName != et.TemplateName)));

        await db.AbacPolicyTemplates.AddRangeAsync(templates
            .Where(nt => existingTemplates.All(et => et.TemplateName != nt.TemplateName)));

        await db.SaveChangesAsync();

        var dbTemplates = await db.AbacPolicyTemplates.AsNoTracking().ToListAsync();
        foreach (var t in dbTemplates)
        {
            await redisDb.StringSetAsync(
                RedisKeys.AbacPolicyTemplate(t.TemplateName),
                JsonSerializer.Serialize(t, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }
}