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
            try
            {
                var keys = server.Keys(pattern: pattern);
                foreach (var key in keys)
                {
                    await redisDb.KeyDeleteAsync(key);
                }
            }
            catch (Exception ex)
            {
                LogError($"删除 Redis Key 模式 {pattern} 失败", ex);
            }
        }

        await DeleteKeysAsync("abac:resource-type:*");
        await DeleteKeysAsync("abac:action:*");
        await DeleteKeysAsync("abac:environment:*");
        await DeleteKeysAsync("abac:policy:*");
        await DeleteKeysAsync("abac:resource-group:*");
        await DeleteKeysAsync("abac:policy-template:*");

        // ---------- 1. ResourceTypes ----------
        try
        {
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
        }
        catch (Exception ex)
        {
            LogError("ResourceTypes 初始化失败", ex);
        }

        // ---------- 2. Actions ----------
        try
        {
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
        }
        catch (Exception ex)
        {
            LogError("Actions 初始化失败", ex);
        }

        // ---------- 3. Environments ----------
        try
        {
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
        }
        catch (Exception ex)
        {
            LogError("Environments 初始化失败", ex);
        }

        // ---------- 4. Policies ----------
        try
        {
            var dbPolicies = await db.AbacPolicies.AsNoTracking().ToListAsync();
            var dbResources = await db.AbacResourceTypes.AsNoTracking().ToListAsync();
            var dbActions = await db.AbacActions.AsNoTracking().ToListAsync();
            var dbEnvs = await db.AbacEnvironments.AsNoTracking().ToListAsync();

            foreach (var p in dbPolicies)
            {
                var resName = dbResources.First(r => r.Id == p.ResourceTypeId).Name;
                var actName = dbActions.First(a => a.Id == p.ActionId).Name;
                var envName = dbEnvs.First(e => e.Id == p.EnvironmentId).Name;

                await redisDb.StringSetAsync(
                    RedisKeys.AbacPolicy(p.TenantId, resName, actName, envName),
                    JsonSerializer.Serialize(p, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            }
        }
        catch (Exception ex)
        {
            LogError("Policies Redis 写入失败", ex);
        }

        // ---------- 5. Resource Groups ----------
        try
        {
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
                foreach (var map in group.ResourceTypes)
                {
                    if (map.ResourceType != null && dbResourceMap.TryGetValue(map.ResourceType.Name, out var existing))
                    {
                        map.ResourceTypeId = existing.Id;
                        map.ResourceType = null;
                    }
                }
            }

            await db.AbacResourceGroups.AddRangeAsync(seedGroups);
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
        }
        catch (Exception ex)
        {
            LogError("ResourceGroups 初始化失败", ex);
        }

        // ---------- 6. Policy Templates ----------
        try
        {
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
        catch (Exception ex)
        {
            LogError("PolicyTemplates 初始化失败", ex);
        }
    }

    private static void LogError(string title, Exception ex)
    {
        Console.WriteLine($"[ABAC:Seed] ❌ {title}: {ex.Message}");

        Exception? inner = ex;
        while (inner?.InnerException != null)
            inner = inner.InnerException;

        if (inner != null && inner != ex)
        {
            Console.WriteLine($"[ABAC:Seed] ❌ 错误详情: {inner.Message}");
        }

        Console.WriteLine(ex.StackTrace);
    }
}
