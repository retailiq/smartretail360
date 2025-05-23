using Microsoft.AspNetCore.Http;

namespace SmartRetail360.Infrastructure.Middlewares;

public class ContextHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public ContextHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Request.Headers;

        void Set(string key, string headerKey)
        {
            if (headers.TryGetValue(headerKey, out var value))
                context.Items[key] = value.ToString();
        }
        
        Set("UserId", "X-User-Id");
        Set("TenantId", "X-Tenant-Id");
        Set("RoleId", "X-Role-Id");
        Set("TraceId", "X-Trace-Id");
        Set("Locale", "X-Locale");
        Set("Email", "X-Email");
        Set("AccountType", "X-Account-Type");
        Set("UserName", "X-User-Name");
        
        // Add TraceId to response headers
        context.Response.OnStarting(() =>
        {
            if (context.Items.TryGetValue("TraceId", out var traceIdObj) && traceIdObj is string traceId)
            {
                context.Response.Headers["X-Trace-Id"] = traceId;
            }
            return Task.CompletedTask;
        });
        
        await _next(context);
        
    }
}