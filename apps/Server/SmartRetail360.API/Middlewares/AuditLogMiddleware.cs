using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.AuditLogging;
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.API.Middlewares;

public class AuditLogMiddleware
{
    private readonly RequestDelegate _next;

    public AuditLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuditLogger auditLogger, IUserContextService userContext)
    {
        var method = context.Request.Method;
        var traceId = userContext.TraceId ?? context.TraceIdentifier;
        var path = context.Request.Path.Value ?? "UNKNOWN";
        var timestamp = DateTime.UtcNow;

        var shouldLog = method is "POST" or "PUT" or "DELETE";

        await _next(context);

        if (!shouldLog) return;

        var statusCode = context.Response.StatusCode;
        var success = statusCode < 400;

        await auditLogger.LogAsync(new AuditContext
        {
            Action = AuditActions.RequestReceived,
            IsSuccess = success,
            Extra = new Dictionary<string, string>
            {
                { "Method", method },
                { "Path", path },
                { "StatusCode", $"{statusCode}" },
                { "ClientIp", userContext.IpAddress ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown" }
            }
        });
    }
}