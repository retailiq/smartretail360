using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Logging.Services.Context;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.API.Middlewares;

public class LoggingContextMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserContextService userContext, ILogContextAccessor accessor)
    {
        // Update Module in the userContext
        var path = context.Request.Path.Value;
        userContext.Module = ModuleResolver.ResolveModule(path);

        // Inject the userContext into the Serilog.Context.LogContext
        using (LogContextEnricher.EnrichFromContext(accessor))
        {
            await _next(context);
        }
    }
}