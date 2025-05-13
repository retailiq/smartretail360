using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.API.Middlewares;

public class AuditLogMiddleware
{
    private readonly RequestDelegate _next;

    public AuditLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditLogger auditLogger, IUserContextService userContext)
    {
        var method = context.Request.Method;
        var traceId = userContext.TraceId ?? context.TraceIdentifier;
        var path = context.Request.Path.Value ?? "UNKNOWN";
        var timestamp = DateTime.UtcNow;

        var shouldLog = method is "POST" or "PUT" or "DELETE";
        var originalStatusCode = 200;

        await _next(context);
        originalStatusCode = context.Response.StatusCode;
        
        if (!shouldLog) return;

        var success = originalStatusCode < 400;

        var audit = new AuditLog
        {
            Action = AuditActions.RequestReceived,
            TraceId = traceId,
            TenantId = userContext.TenantId ?? null,
            UserId = userContext.UserId ?? null,
            EvaluatedAt = timestamp,
            IsSuccess = success,
            UnserializedDetails = new Dictionary<string, string>
            {
                { "Method", method },
                { "Path", path },
                { "StatusCode", $"{originalStatusCode}" },
                { "ClientIp", userContext.IpAddress ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown" }
            }
        };

        await auditLogger.LogAsync(audit);
    }
}