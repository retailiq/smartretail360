using System.Text.Json;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Data;

namespace SmartRetail360.Infrastructure.Services.Common;


public class AuditLogger : IAuditLogger
{
    private readonly AppDbContext _dbContext;

    public AuditLogger(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAsync(AuditLog entry)
    {
        // 自动将 Details 字典序列化为 JSON 字符串存入 DetailsJson 字段
        if (entry.DetailsJson == null && entry.UnserializedDetails != null)
        {
            entry.DetailsJson = JsonSerializer.Serialize(
                entry.UnserializedDetails,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }

        _dbContext.AuditLogs.Add(entry);
        await _dbContext.SaveChangesAsync();
    }
}