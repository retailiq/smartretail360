using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;
using System.Text.Json;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Infrastructure.Logging.Loggers;

public class AuditLogger : IAuditLogger
{
    private readonly AppDbContext _db;
    private readonly ILogContextAccessor _context;
    private readonly ISafeExecutor _executor;

    public AuditLogger(
        AppDbContext db,
        ILogContextAccessor context,
        ISafeExecutor executor)
    {
        _db = db;
        _context = context;
        _executor = executor;

    }

    public async Task LogAsync(AuditContext ctx)
    {
        var details = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(ctx.Email)) details["Email"] = ctx.Email;
        if (!string.IsNullOrWhiteSpace(ctx.ErrorStack)) details["ErrorStack"] = ctx.ErrorStack;
        if (!string.IsNullOrWhiteSpace(ctx.Reason)) details["Reason"] = ctx.Reason;

        details["SourceIp"] = _context.IpAddress ?? GeneralConstants.Unknown;

        var log = new AuditLog
        {
            LogId = ctx.LogId,
            Action = ctx.Action,
            IsSuccess = ctx.IsSuccess,
            TenantId = ctx.TenantId,
            UserId = ctx.UserId,
            EvaluatedAt = DateTime.UtcNow,
            TraceId = _context.TraceId ?? Guid.NewGuid().ToString(),
            UnserializedDetails = details,
            Level = ctx.Level,
            SourceModule = ctx.SourceModule,
            DetailsJson = JsonSerializer.Serialize(details),
            Category = ctx.Category ?? LogCategory.Application
        };

        await _executor.ExecuteAsync(async () =>
            {
                _db.AuditLogs.Add(log);
                await _db.SaveChangesAsync();
            },
            LogEventType.WriteLogFailure,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );
    }
}