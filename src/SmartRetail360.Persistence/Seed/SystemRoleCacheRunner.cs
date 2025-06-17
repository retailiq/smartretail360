using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using SmartRetail360.Shared.Redis;
using System.Text.Json;
using SmartRetail360.Persistence.Data;

namespace SmartRetail360.Persistence.Seed;

public static class SystemRoleCacheRunner
{
    public static async Task RunAsync(AppDbContext db, IConnectionMultiplexer redis)
    {
        var redisDb = redis.GetDatabase();

        var allRoles = await db.Roles
            .AsNoTracking()
            .ToListAsync();
        
        var anyMissing = false;
        
        foreach (var role in allRoles)
        {
            var key = RedisKeys.SystemRole(role.Name);
            var exists = await redisDb.KeyExistsAsync(key);
            
            if (!exists)
            {
                var json = JsonSerializer.Serialize(role);
                await redisDb.StringSetAsync(key, json);
                anyMissing = true;
            }
        }
        
        if (!anyMissing)
        {
            Console.WriteLine("[Seed] System roles already exist in Redis. Skipped.");
        }
    }
}