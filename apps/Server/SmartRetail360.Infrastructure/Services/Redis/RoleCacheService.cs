using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Caching;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Redis;
using SmartRetail360.Shared.Utils;
using Role = SmartRetail360.Domain.Entities.Role;
using StackExchange.Redis;
using System.Text.Json;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Contracts.AccountRegistration.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Redis;

public class RoleCacheService : IRoleCacheService
{
    private readonly AppDbContext _db;
    private readonly IDatabase _redis;
    private readonly ISafeExecutor _safeExecutor;
    private readonly ILogDispatcher _logDispatcher;
    private readonly IUserContextService _userContext;

    public RoleCacheService(
        AppDbContext db,
        IConnectionMultiplexer redis,
        ISafeExecutor safeExecutor,
        ILogDispatcher logDispatcher,
        IUserContextService userContext)
    {
        _db = db;
        _redis = redis.GetDatabase();
        _safeExecutor = safeExecutor;
        _logDispatcher = logDispatcher;
        _userContext = userContext;
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
                _userContext.Inject(
                    errorStack: ex.ToString()
                );
                await _logDispatcher.Dispatch(
                    LogEventType.RegisterFailure,
                    LogReasons.RoleDeserializationFailed
                );

                await _redis.KeyDeleteAsync(cacheKey);
            }
        }

        // If not found in cache, fetch from the database
        var roleResult = await _safeExecutor.ExecuteAsync(
            () => _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Name == roleName),
            LogEventType.RegisterFailure,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );

        if (!roleResult.IsSuccess || roleResult.Response.Data == null)
            return null;

        var role = roleResult.Response.Data;
        await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(role), TimeSpan.FromHours(24));
        return role;
    }
}