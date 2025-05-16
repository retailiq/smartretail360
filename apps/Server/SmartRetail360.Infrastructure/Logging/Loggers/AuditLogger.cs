using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;
using System.Text.Json;

namespace SmartRetail360.Infrastructure.Logging.Loggers;

public class AuditLogger : IAuditLogger
{
    private readonly AppDbContext _db;
    private readonly IUserContextService _userContext;

    public AuditLogger(AppDbContext db, IUserContextService userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public async Task LogAsync(AuditContext ctx)
    {
        var details = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(ctx.Email)) details["Email"] = ctx.Email;
        if (!string.IsNullOrWhiteSpace(ctx.ErrorStack)) details["ErrorStack"] = ctx.ErrorStack;
        if (!string.IsNullOrWhiteSpace(ctx.Reason)) details["Reason"] = ctx.Reason;

        details["SourceIp"] = _userContext.IpAddress ?? "unknown";

        var log = new AuditLog
        {
            LogId = ctx.LogId,
            Action = ctx.Action,
            IsSuccess = ctx.IsSuccess,
            TraceId = _userContext.TraceId ?? Guid.NewGuid().ToString(),
            TenantId = _userContext.TenantId,
            UserId = _userContext.UserId,
            EvaluatedAt = DateTime.UtcNow,
            UnserializedDetails = details,
            Level = ctx.Level,
            SourceModule = ctx.SourceModule,
            DetailsJson = JsonSerializer.Serialize(details)
        };

        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}