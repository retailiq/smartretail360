using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using StackExchange.Redis;
using SmartRetail360.Application.Interfaces.Caching;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Redis;

namespace SmartRetail360.Infrastructure.Services.Redis;

public class ActivationTokenCacheService : IActivationTokenCacheService
{
    private readonly IDatabase _redis;
    private readonly AppDbContext _db;
    private readonly ILogDispatcher _logDispatcher;
    private readonly IUserContextService _userContext;
    private readonly ISafeExecutor _safeExecutor;
    private readonly AppOptions _options;

    public ActivationTokenCacheService(
        IConnectionMultiplexer redis,
        AppDbContext db,
        ILogDispatcher logDispatcher,
        IUserContextService userContext,
        ISafeExecutor safeExecutor,
        IOptions<AppOptions> options)
    {
        _redis = redis.GetDatabase();
        _db = db;
        _logDispatcher = logDispatcher;
        _userContext = userContext;
        _safeExecutor = safeExecutor;
        _options = options.Value;
    }

    public async Task SetTokenAsync(AccountActivationToken tokenEntity)
    {
        var key = RedisKeys.ActivationToken(tokenEntity.Token);
        var json = JsonSerializer.Serialize(tokenEntity);
        var ttl = TimeSpan.FromMinutes(_options.ActivationTokenLimitMinutes);
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
                await _logDispatcher.Dispatch(LogEventType.AccountActivateFailure,
                    LogReasons.TokenDeserializationFailed);
                await _redis.KeyDeleteAsync(key);
            }
        }

        // If not found in Redis, check the database
        var roleResult = await _safeExecutor.ExecuteAsync(
            () => _db.AccountActivationTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Token == token),
            LogEventType.RedisError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );

        var tokenEntity = roleResult.Response.Data;

        if (!roleResult.IsSuccess || roleResult.Response.Data == null)
            return null;

        // If found in the database, set it in Redis for future requests
        var remainingTtl = tokenEntity!.ExpiresAt - DateTime.UtcNow;
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