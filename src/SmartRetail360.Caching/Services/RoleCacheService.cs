using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Caching.Interfaces.Caching;
using SmartRetail360.Execution;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Redis;
using SmartRetail360.Shared.Utils;
using StackExchange.Redis;
using Role = SmartRetail360.Domain.Entities.Role;

namespace SmartRetail360.Caching.Services;

public class RoleCacheService : IRoleCacheService
{
    private readonly AppDbContext _db;
    private readonly IDatabase _redis;
    private readonly IUserContextService _userContext;
    private readonly ISafeExecutor  _safeExecutor;

    public RoleCacheService(
        AppDbContext db,
        IConnectionMultiplexer redis,
        IUserContextService userContext,
        ISafeExecutor safeExecutor)
    {
        _db = db;
        _redis = redis.GetDatabase();
        _userContext = userContext;
        _safeExecutor = safeExecutor;
    }

    public async Task<Role?> GetSystemRoleAsync(SystemRoleType roleType)
    {
        var roleName = RoleHelper.GetRoleName(roleType);
        var cacheKey = RedisKeys.SystemRole(roleName);

        // Get the role from cache first
        var cached = await _redis.StringGetAsync(cacheKey);
        if (cached.HasValue)
        {
            try
            {
                return JsonSerializer.Deserialize<Role>(cached!);
            }
            catch (Exception ex)
            {
                _userContext.Inject(new UserExecutionContext { ErrorStack = ex.ToString() });
                await _redis.KeyDeleteAsync(cacheKey);
            }
        }

        // If not found in cache, fetch from the database
        var roleResult = await _safeExecutor.ExecuteAsync(
            () => _db.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == roleName),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        var role = roleResult.Response.Data;

        if (role == null)
            return null;

        await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(role), TimeSpan.FromHours(24));
        return role;
    }

    public async Task<List<Role>> GetSystemRolesByIdsAsync(List<Guid> roleIds)
    {
        var roles = new List<Role>();
        var missedIds = new List<Guid>();

        foreach (var roleId in roleIds)
        {
            var cacheKey = RedisKeys.SystemRole(roleId.ToString());
            var cached = await _redis.StringGetAsync(cacheKey);

            if (cached.HasValue)
            {
                try
                {
                    var role = JsonSerializer.Deserialize<Role>(cached!);
                    if (role != null)
                    {
                        roles.Add(role);
                        continue;
                    }
                }
                catch
                {
                    // ignore and fallback to DB
                }
            }

            missedIds.Add(roleId); // cache miss or deserialization fail
        }

        // fallback to the database for missed roles
        if (missedIds.Count > 0)
        {
            var dbRoles = await _db.Roles
                .AsNoTracking()
                .Where(r => missedIds.Contains(r.Id))
                .ToListAsync();

            if (dbRoles.Count > 0)
            {
                roles.AddRange(dbRoles);

                foreach (var role in dbRoles)
                {
                    var key = RedisKeys.SystemRole(role.Id.ToString());
                    await _redis.StringSetAsync(key, JsonSerializer.Serialize(role), TimeSpan.FromHours(24));
                }
            }
        }

        return roles;
    }
}