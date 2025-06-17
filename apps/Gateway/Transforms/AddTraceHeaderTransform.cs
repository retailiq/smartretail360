using SmartRetail360.Shared.Contexts.Resolvers;
using Yarp.ReverseProxy.Transforms;

namespace SmartRetail360.Gateway.Transforms;

public class AddTraceHeaderTransform : RequestTransform
{
    public override ValueTask ApplyAsync(RequestTransformContext context)
    {
        var httpContext = context.HttpContext;
        
        var traceId = Guid.NewGuid().ToString("N");
        context.ProxyRequest.Headers.Remove("X-Trace-Id");
        context.ProxyRequest.Headers.Add("X-Trace-Id", traceId);
        
        var path = httpContext.Request.Path.Value?.ToLowerInvariant() ?? "";
        var env = EnvResolver.ResolveEnv(path);
        context.ProxyRequest.Headers.Remove("X-Env");
        context.ProxyRequest.Headers.Add("X-Env", env);
        
        return ValueTask.CompletedTask;
    }
}