using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Infrastructure.AuditLogging;

public class AuditLogger
{
    private readonly AppDbContext _db;
    private readonly IUserContextService _userContext;

    public AuditLogger(AppDbContext db, IUserContextService userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public Task LogRegisterSuccessAsync(string email) =>
        LogAsync(new AuditContext
        {
            Action = AuditActions.RegisterTenant,
            IsSuccess = true,
            Email = email
        });

    public Task LogAccountExistsAsync(string email) =>
        LogAsync(new AuditContext
        {
            Action = AuditActions.RegisterTenant,
            IsSuccess = false,
            Email = email,
            Reason = AuditLogReasons.TenantAlreadyExists,
            ReasonMessage = AuditLogReasons.Descriptions[AuditLogReasons.TenantAlreadyExists]
        });

    public Task LogLockFailedAsync(string email) =>
        LogAsync(new AuditContext
        {
            Action = AuditActions.GenerateAccountLockFailed,
            IsSuccess = false,
            Email = email,
            Reason = AuditLogReasons.LockNotAcquired,
            ReasonMessage = AuditLogReasons.Descriptions[AuditLogReasons.LockNotAcquired]
        });

    public Task LogEmailFailedAsync(string email, string error) =>
        LogAsync(new AuditContext
        {
            Action = AuditActions.SendEmailFailed,
            IsSuccess = false,
            Email = email,
            Template = nameof(EmailTemplate.TenantAccountActivation),
            Error = error
        });

    public async Task LogAsync(AuditContext ctx)
    {
        var details = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(ctx.Email)) details["Email"] = ctx.Email;
        if (!string.IsNullOrWhiteSpace(ctx.Error)) details["Error"] = ctx.Error;
        if (!string.IsNullOrWhiteSpace(ctx.Template)) details["Template"] = ctx.Template;
        if (!string.IsNullOrWhiteSpace(ctx.Reason)) details["Reason"] = ctx.Reason;
        if (!string.IsNullOrWhiteSpace(ctx.ReasonMessage)) details["ReasonMessage"] = ctx.ReasonMessage;
        if (ctx.Extra != null)
        {
            foreach (var kv in ctx.Extra)
                details[kv.Key] = kv.Value;
        }

        details["SourceIp"] = _userContext.IpAddress ?? "unknown";

        var log = new AuditLog
        {
            Action = ctx.Action,
            IsSuccess = ctx.IsSuccess,
            TraceId = _userContext.TraceId ?? Guid.NewGuid().ToString(),
            TenantId = _userContext.TenantId,
            UserId = _userContext.UserId,
            EvaluatedAt = DateTime.UtcNow,
            UnserializedDetails = details,
            DetailsJson = System.Text.Json.JsonSerializer.Serialize(details)
        };

        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}