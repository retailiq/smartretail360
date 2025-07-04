using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Logging;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Execution;
using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Logging.Services.Loggers;

public class AuditLogger : IAuditLogger
{
    private readonly ILogContextAccessor _context;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IServiceProvider _serviceProvider;

    public AuditLogger(
        ILogContextAccessor context,
        ISafeExecutor safeExecutor,
        IServiceProvider serviceProvider)
    {
        _context = context;
        _safeExecutor = safeExecutor;
        _serviceProvider = serviceProvider;
    }

    public async Task LogAsync(AuditContext ctx)
    {
        var details = new Dictionary<string, string>();
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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
        
        await _safeExecutor.ExecuteAsync(
            async () =>
            {
                db.AuditLogs.Add(log);
                await db.SaveChangesAsync();
            },
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
    }
}