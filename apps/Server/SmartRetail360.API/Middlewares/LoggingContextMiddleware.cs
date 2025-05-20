using SmartRetail360.Application.Common;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Infrastructure.Logging.Context;
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

        // Inject the userContext into the Serilog'LogContext
        using (LogContextEnricher.EnrichFromContext(accessor))
        {
            await _next(context);
        }
    }
}