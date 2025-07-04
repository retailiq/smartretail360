using SmartRetail360.Shared.Contexts.Resolvers;

namespace SmartRetail360.Gateway.Middlewares;

public class RequestContextEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public RequestContextEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Guid.NewGuid().ToString("N");
        context.Request.Headers["X-Trace-Id"] = traceId;
        context.Items["TraceId"] = traceId;
        
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        var env = EnvResolver.ResolveEnv(path);
        context.Request.Headers["X-Env"] = env;
        context.Items["Env"] = env;
        
        await _next(context);
    }
}