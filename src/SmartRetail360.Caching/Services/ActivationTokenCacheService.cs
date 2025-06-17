using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartRetail360.Caching.Interfaces.Caching;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Execution;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Redis;
using StackExchange.Redis;

namespace SmartRetail360.Caching.Services;

public class ActivationTokenCacheService : IActivationTokenCacheService
{
    private readonly IDatabase _redis;
    private readonly AppDbContext _db;
    private readonly IUserContextService _userContext;
    private readonly AppOptions _options;
    private readonly ISafeExecutor  _safeExecutor;

    public ActivationTokenCacheService(
        IConnectionMultiplexer redis,
        AppDbContext db,
        IUserContextService userContext,
        ISafeExecutor safeExecutor,
        IOptions<AppOptions> options)
    {
        _redis = redis.GetDatabase();
        _db = db;
        _userContext = userContext;
        _options = options.Value;
        _safeExecutor = safeExecutor;
    }

    public async Task SetTokenAsync(AccountActivationToken tokenEntity)
    {
        var key = RedisKeys.ActivationToken(tokenEntity.Token);
        var json = JsonSerializer.Serialize(tokenEntity);
        var ttl = TimeSpan.FromMinutes(_options.AccountActivationLimitMinutes);
        await _redis.StringSetAsync(key, json, ttl);
    }

    public async Task<AccountActivationToken?> GetTokenAsync(string token)
    {
        var key = RedisKeys.ActivationToken(token);

        // Check if the token exists in Redis
        var json = await _redis.StringGetAsync(key);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                return JsonSerializer.Deserialize<AccountActivationToken>(json!);
            }
            catch (Exception ex)
            {
                // Log the error and remove the invalid token from Redis
                _userContext.Inject(new UserExecutionContext { ErrorStack = ex.ToString() });
                await _redis.KeyDeleteAsync(key);
            }
        }
        
        var tokenEntityResult = await _safeExecutor.ExecuteAsync(
            () => _db.AccountActivationTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Token == token),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        var tokenEntity = tokenEntityResult.Response.Data;

        if (tokenEntity == null)
            return null;
        
        // If found in the database, set it in Redis for future requests
        var remainingTtl = tokenEntity.ExpiresAt - DateTime.UtcNow;
        if (remainingTtl <= TimeSpan.Zero)
            return tokenEntity;

        var serializedToken = JsonSerializer.Serialize(tokenEntity);
        await _redis.StringSetAsync(key, serializedToken, remainingTtl);
        
        // Attach the token entity to the DbContext to update its status
        _db.Attach(tokenEntity);
        
        return tokenEntity;
    }

    // Delete the token
    public async Task InvalidateTokenAsync(string token)
    {
        var key = RedisKeys.ActivationToken(token);
        await _redis.KeyDeleteAsync(key);
    }
}