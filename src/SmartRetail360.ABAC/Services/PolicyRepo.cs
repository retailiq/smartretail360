using Microsoft.EntityFrameworkCore;
using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Redis;
using StackExchange.Redis;
using System.Text.Json;
using SmartRetail360.Execution;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.ABAC.Services;

public class PolicyRepo : IPolicyRepo
{
    private readonly AppDbContext _db;
    private readonly IDatabase _redisDb;
    private readonly ISafeExecutor _safeExecutor;

    public PolicyRepo(
        AppDbContext db,
        IConnectionMultiplexer redis,
        ISafeExecutor safeExecutor)
    {
        _db = db;
        _redisDb = redis.GetDatabase();
        _safeExecutor = safeExecutor;
    }

    // Get policy JSON by composite key
    public async Task<string?> GetPolicyJsonAsync(Guid tenantId, string resourceType, string action)
    {
        var policies = await _redisDb.SetMembersAsync(RedisKeys.AbacPolicyIndex);
        foreach (var policy in policies)
        {
            if (policy != $"{tenantId}:{resourceType}:{action}")
                continue;

            var redisKey = RedisKeys.AbacPolicy(tenantId, resourceType, action);
            var cached = await _redisDb.StringGetAsync(redisKey);
            if (cached.HasValue)
            {
                var item = JsonSerializer.Deserialize<AbacPolicy>(cached!, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (item != null && item.IsEnabled)
                {
                    Console.WriteLine("[ABAC in Policy Repo] Current JSON：" + item.RuleJson);
                    return item.RuleJson;
                }
            }
        }

        var dbPolicyResult = await _safeExecutor.ExecuteAsync(
            () => _db.AbacPolicies
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.ResourceType.Name == resourceType &&
                    p.Action.Name == action &&
                    p.TenantId == tenantId &&
                    p.IsEnabled),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.None
        );

        var dbPolicy = dbPolicyResult.Response.Data;

        if (dbPolicy != null)
        {
            await _redisDb.StringSetAsync(
                RedisKeys.AbacPolicy(tenantId, resourceType, action),
                JsonSerializer.Serialize(dbPolicy, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));

            await _redisDb.SetAddAsync(RedisKeys.AbacPolicyIndex, $"{tenantId}:{resourceType}:{action}");

            if (dbPolicy.IsEnabled)
                return dbPolicy.RuleJson;
        }

        return null;
    }

    // Get all enabled policy templates
    public async Task<List<AbacPolicyTemplate>> GetPolicyTemplatesMapAsync()
    {
        var list = new List<AbacPolicyTemplate>();
        var templateNames = await _redisDb.SetMembersAsync(RedisKeys.AbacPolicyTemplateIndex);
        foreach (var name in templateNames)
        {
            var redisKey = RedisKeys.AbacPolicyTemplate(name!);
            var cached = await _redisDb.StringGetAsync(redisKey);
            if (cached.HasValue)
            {
                var item = JsonSerializer.Deserialize<AbacPolicyTemplate>(cached!, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (item != null)
                    list.Add(item);
            }
        }

        if (list.Count > 0)
            return list;

        var dbTemplatesResult = await _safeExecutor.ExecuteAsync(
            () => _db.AbacPolicyTemplates.AsNoTracking()
                .Where(t => t.IsEnabled).ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.None
        );
        var dbTemplates = dbTemplatesResult.Response.Data;

        if (dbTemplates != null)
        {
            foreach (var t in dbTemplates)
            {
                await _redisDb.StringSetAsync(
                    RedisKeys.AbacPolicyTemplate(t.TemplateName),
                    JsonSerializer.Serialize(t,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

                await _redisDb.SetAddAsync(RedisKeys.AbacPolicyTemplateIndex, t.TemplateName);
                list.Add(t);
            }
        }

        return list;
    }

    // Get all environments in a dictionary
    public async Task<Dictionary<string, Guid>> GetAllEnvironmentMapAsync()
    {
        var map = new Dictionary<string, Guid>();
        var names = await _redisDb.SetMembersAsync(RedisKeys.AbacEnvironmentIndex);
        foreach (var name in names)
        {
            var redisKey = RedisKeys.AbacEnvironment(name!);
            var cached = await _redisDb.StringGetAsync(redisKey);
            if (cached.HasValue)
            {
                var entity = JsonSerializer.Deserialize<AbacEnvironment>(cached!, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (entity != null)
                    map[entity.Name] = entity.Id;
            }
        }

        if (map.Count > 0)
            return map;

        var envsResult = await _safeExecutor.ExecuteAsync(
            () => _db.AbacEnvironments.AsNoTracking().ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.None
        );
        var envs = envsResult.Response.Data;
        if (envs != null)
        {
            foreach (var e in envs)
            {
                await _redisDb.StringSetAsync(RedisKeys.AbacEnvironment(e.Name),
                    JsonSerializer.Serialize(e,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
                await _redisDb.SetAddAsync(RedisKeys.AbacEnvironmentIndex, e.Name);
                map[e.Name] = e.Id;
            }
        }

        return map;
    }

    // Get all resource types in a dictionary
    public async Task<Dictionary<string, Guid>> GetAllResourceTypeMapAsync()
    {
        var map = new Dictionary<string, Guid>();
        var names = await _redisDb.SetMembersAsync(RedisKeys.AbacResourceTypeIndex);
        foreach (var name in names)
        {
            var redisKey = RedisKeys.AbacResourceType(name!);
            var cached = await _redisDb.StringGetAsync(redisKey);
            if (cached.HasValue)
            {
                var entity = JsonSerializer.Deserialize<AbacResourceType>(cached!, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (entity != null)
                    map[entity.Name] = entity.Id;
            }
        }

        if (map.Count > 0)
            return map;

        var typesResult = await _safeExecutor.ExecuteAsync(
            () => _db.AbacResourceTypes.AsNoTracking().ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.None
        );
        var types = typesResult.Response.Data;

        if (types != null)
        {
            foreach (var t in types)
            {
                await _redisDb.StringSetAsync(RedisKeys.AbacResourceType(t.Name),
                    JsonSerializer.Serialize(t,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
                await _redisDb.SetAddAsync(RedisKeys.AbacResourceTypeIndex, t.Name);
                map[t.Name] = t.Id;
            }
        }

        return map;
    }

    // Get all resource groups and their mapped types
    public async Task<Dictionary<string, List<string>>> GetAllResourceGroupMapAsync()
    {
        var map = new Dictionary<string, List<string>>();
        var names = await _redisDb.SetMembersAsync(RedisKeys.AbacResourceGroupIndex);
        foreach (var name in names)
        {
            var redisKey = RedisKeys.AbacResourceGroup(name!);
            var cached = await _redisDb.StringGetAsync(redisKey);
            if (cached.HasValue)
            {
                var parsed = JsonDocument.Parse((string)cached!);
                var groupName = parsed.RootElement.GetProperty("name").GetString();
                var resourceTypes = parsed.RootElement.GetProperty("resourceTypes")
                    .EnumerateArray().Select(rt => rt.GetString()!).ToList();

                if (!string.IsNullOrWhiteSpace(groupName))
                    map[groupName] = resourceTypes;
            }
        }

        if (map.Count > 0)
            return map;

        var groupsResult = await _safeExecutor.ExecuteAsync(
            () => _db.AbacResourceGroups
                .Include(g => g.ResourceTypes)
                .ThenInclude(m => m.ResourceType)
                .AsNoTracking().ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.None
        );
        var groups = groupsResult.Response.Data;

        if (groups != null)
        {
            foreach (var g in groups)
            {
                var redisKey = RedisKeys.AbacResourceGroup(g.Name);
                var value = new
                {
                    g.Id,
                    g.Name,
                    ResourceTypes = g.ResourceTypes.Select(rt => rt.ResourceType.Name).ToList()
                };
                await _redisDb.StringSetAsync(redisKey,
                    JsonSerializer.Serialize(value,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
                await _redisDb.SetAddAsync(RedisKeys.AbacResourceGroupIndex, g.Name);

                map[g.Name] = value.ResourceTypes;
            }
        }

        return map;
    }

    // Get all actions in a dictionary
    public async Task<Dictionary<string, Guid>> GetAllActionMapAsync()
    {
        var map = new Dictionary<string, Guid>();
        var names = await _redisDb.SetMembersAsync(RedisKeys.AbacActionIndex);
        foreach (var name in names)
        {
            var redisKey = RedisKeys.AbacAction(name!);
            var cached = await _redisDb.StringGetAsync(redisKey);
            if (cached.HasValue)
            {
                var entity = JsonSerializer.Deserialize<AbacAction>(cached!, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (entity != null)
                    map[entity.Name.ToLowerInvariant()] = entity.Id;
            }
        }

        if (map.Count > 0)
            return map;

        var actionsResult = await _safeExecutor.ExecuteAsync(
            () => _db.AbacActions.AsNoTracking().ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.None
        );
        var actions = actionsResult.Response.Data;

        if (actions != null)
        {
            foreach (var a in actions)
            {
                await _redisDb.StringSetAsync(RedisKeys.AbacAction(a.Name),
                    JsonSerializer.Serialize(a,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
                await _redisDb.SetAddAsync(RedisKeys.AbacActionIndex, a.Name);
                map[a.Name.ToLowerInvariant()] = a.Id;
            }
        }

        return map;
    }
}