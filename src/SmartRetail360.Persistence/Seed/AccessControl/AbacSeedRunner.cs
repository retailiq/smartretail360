using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Redis;
using StackExchange.Redis;

namespace SmartRetail360.Persistence.Seed.AccessControl;

public static class AbacSeedRunner
{
    public static async Task RunAsync(AppDbContext db, IConnectionMultiplexer redis)
    {
        var redisDb = redis.GetDatabase();
        var server = redis.GetServer(redis.GetEndPoints()[0]);

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
            var seedResources = AbacPolicyResourceTypeSeeder.GetResourceTypes().ToList();
            var existingResources = await db.AbacResourceTypes.ToListAsync();

            // 删除：数据库中有但种子中没有的
            var toDelete = existingResources
                .Where(er => seedResources.All(sr => sr.Name != er.Name))
                .ToList();
            db.AbacResourceTypes.RemoveRange(toDelete);

            // 新增：种子中有但数据库没有的
            var toAdd = seedResources
                .Where(sr => existingResources.All(er => er.Name != sr.Name))
                .ToList();
            await db.AbacResourceTypes.AddRangeAsync(toAdd);

            await db.SaveChangesAsync();

            // 写入 Redis 缓存（全量）
            var dbResources = await db.AbacResourceTypes.AsNoTracking().ToListAsync();
            foreach (var r in dbResources)
            {
                var key = RedisKeys.AbacResourceType(r.Name);
                await redisDb.StringSetAsync(
                    key,
                    JsonSerializer.Serialize(r,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

                await redisDb.SetAddAsync(RedisKeys.AbacResourceTypeIndex, r.Name); // ✅ 加索引
            }
        }
        catch (Exception ex)
        {
            LogError("ResourceTypes 初始化失败", ex);
        }

        // ---------- 2. Actions ----------
        try
        {
            var seedActions = AbacPolicyActionSeeder.GetActions().ToList();
            var existingActions = await db.AbacActions.ToListAsync();

            // 删除：数据库中存在但种子中没有的
            var toDelete = existingActions
                .Where(ea => seedActions.All(sa => sa.Name != ea.Name))
                .ToList();
            db.AbacActions.RemoveRange(toDelete);

            // 新增：种子中存在但数据库中没有的
            var toAdd = seedActions
                .Where(sa => existingActions.All(ea => ea.Name != sa.Name))
                .ToList();
            await db.AbacActions.AddRangeAsync(toAdd);

            await db.SaveChangesAsync();

            // 写入 Redis 缓存（全量）
            var dbActions = await db.AbacActions.AsNoTracking().ToListAsync();
            foreach (var a in dbActions)
            {
                var key = RedisKeys.AbacAction(a.Name);
                await redisDb.StringSetAsync(
                    key,
                    JsonSerializer.Serialize(a,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

                await redisDb.SetAddAsync(RedisKeys.AbacActionIndex, a.Name); // ✅ 加索引
            }
        }
        catch (Exception ex)
        {
            LogError("Actions 初始化失败", ex);
        }

        // ---------- 3. Environments ----------
        try
        {
            var seedEnvs = AbacPolicyEnvironmentSeeder.GetEnvironments().ToList();
            var existingEnvs = await db.AbacEnvironments.ToListAsync();

            // 删除：数据库中存在但种子中没有的
            var toDelete = existingEnvs
                .Where(ee => seedEnvs.All(se => se.Name != ee.Name))
                .ToList();
            db.AbacEnvironments.RemoveRange(toDelete);

            // 新增：种子中存在但数据库中没有的
            var toAdd = seedEnvs
                .Where(se => existingEnvs.All(ee => ee.Name != se.Name))
                .ToList();
            await db.AbacEnvironments.AddRangeAsync(toAdd);

            await db.SaveChangesAsync();

            // 写入 Redis 缓存（全量）
            var dbEnvs = await db.AbacEnvironments.AsNoTracking().ToListAsync();
            foreach (var e in dbEnvs)
            {
                var key = RedisKeys.AbacEnvironment(e.Name);
                await redisDb.StringSetAsync(
                    key,
                    JsonSerializer.Serialize(e,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

                await redisDb.SetAddAsync(RedisKeys.AbacEnvironmentIndex, e.Name); // ✅ 加索引
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

            foreach (var p in dbPolicies)
            {
                var resName = dbResources.First(r => r.Id == p.ResourceTypeId).Name;
                var actName = dbActions.First(a => a.Id == p.ActionId).Name;

                // 写入 Redis 缓存（全量）
                await redisDb.StringSetAsync(
                    RedisKeys.AbacPolicy(p.TenantId, resName, actName),
                    JsonSerializer.Serialize(p,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

                await redisDb.SetAddAsync(RedisKeys.AbacPolicyIndex,
                    $"{p.TenantId}:{resName}:{actName}");
            }
        }
        catch (Exception ex)
        {
            LogError("Policies Redis 写入失败", ex);
        }

        // ---------- 5. Resource Groups ----------
        try
        {
            var now = DateTime.UtcNow;
            var seedGroups = AbacPolicyResourceGroupSeeder.GetResourceGroups().ToList();
            var existingGroups = await db.AbacResourceGroups
                .Include(g => g.ResourceTypes)
                .ToListAsync();

            var dbResourceMap = await db.AbacResourceTypes.ToDictionaryAsync(r => r.Name);

            // 1. 删除数据库中存在但种子中没有的 group
            var toDelete = existingGroups
                .Where(g => seedGroups.All(sg => sg.Name != g.Name))
                .ToList();
            db.AbacResourceGroups.RemoveRange(toDelete);

            // 2. 遍历种子 group：如果存在就更新 ResourceTypes，若不存在就新增
            foreach (var seed in seedGroups)
            {
                var existing = existingGroups.FirstOrDefault(g => g.Name == seed.Name);

                // 将 ResourceType.Name 映射为 ResourceTypeId
                foreach (var map in seed.ResourceTypes)
                {
                    if (map.ResourceType != null && dbResourceMap.TryGetValue(map.ResourceType.Name, out var matched))
                    {
                        map.ResourceTypeId = matched.Id;
                        map.ResourceType = null;
                    }
                }

                if (existing == null)
                {
                    // 新增 group
                    db.AbacResourceGroups.Add(seed);
                }
                else
                {
                    // 差异更新 ResourceTypes（先删后加）
                    var existingMapIds = existing.ResourceTypes.Select(rt => rt.ResourceTypeId).ToHashSet();
                    var newMapIds = seed.ResourceTypes.Select(rt => rt.ResourceTypeId).ToHashSet();

                    // 删除多余
                    var toRemove = existing.ResourceTypes
                        .Where(rt => !newMapIds.Contains(rt.ResourceTypeId))
                        .ToList();
                    foreach (var r in toRemove)
                        existing.ResourceTypes.Remove(r);

                    // 新增缺失
                    var toAdd = seed.ResourceTypes
                        .Where(rt => !existingMapIds.Contains(rt.ResourceTypeId))
                        .ToList();
                    foreach (var a in toAdd)
                        existing.ResourceTypes.Add(a);

                    existing.UpdatedAt = now;
                }
            }

            await db.SaveChangesAsync();

            // 写入 Redis 缓存（部分）
            var dbGroups = await db.AbacResourceGroups
                .Include(g => g.ResourceTypes)
                .ThenInclude(m => m.ResourceType)
                .AsNoTracking()
                .ToListAsync();

            foreach (var g in dbGroups)
            {
                var key = RedisKeys.AbacResourceGroup(g.Name);
                await redisDb.StringSetAsync(
                    key,
                    JsonSerializer.Serialize(new
                    {
                        g.Id,
                        g.Name,
                        ResourceTypes = g.ResourceTypes.Select(rt => rt.ResourceType.Name).ToList()
                    }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

                await redisDb.SetAddAsync(RedisKeys.AbacResourceGroupIndex, g.Name); // ✅ 加索引
            }
        }
        catch (Exception ex)
        {
            LogError("ResourceGroups 初始化失败", ex);
        }

        // ---------- 6. Policy Templates ----------
        try
        {
            var newTemplates = AbacPolicyTemplateSeeder.GetTemplates().ToList();
            var existingTemplates = await db.AbacPolicyTemplates.ToListAsync();

            foreach (var newTemplate in newTemplates)
            {
                var existing = existingTemplates.FirstOrDefault(x => x.TemplateName == newTemplate.TemplateName);

                if (existing != null)
                {
                    // 更新已有模板（保留原 TemplateId）
                    existing.RuleJson = newTemplate.RuleJson;
                    existing.Action = newTemplate.Action;
                    existing.ResourceType = newTemplate.ResourceType;
                    existing.IsEnabled = newTemplate.IsEnabled;
                }
                else
                {
                    // 新增新模板
                    db.AbacPolicyTemplates.Add(newTemplate);
                }
            }

            await db.SaveChangesAsync();

            // 写入 Redis 缓存（全量）
            var dbTemplates = await db.AbacPolicyTemplates.AsNoTracking().ToListAsync();
            foreach (var t in dbTemplates)
            {
                await redisDb.StringSetAsync(
                    RedisKeys.AbacPolicyTemplate(t.TemplateName),
                    JsonSerializer.Serialize(t,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

                // ✅ 添加索引集合（Set），方便之后批量获取
                await redisDb.SetAddAsync(RedisKeys.AbacPolicyTemplateIndex, t.TemplateName);
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