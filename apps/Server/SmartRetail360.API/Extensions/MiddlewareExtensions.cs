using SmartRetail360.API.Middlewares;
using SmartRetail360.Infrastructure.Middlewares;

namespace SmartRetail360.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseSmartRetailMiddlewares(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<ContextHeaderMiddleware>()
            .UseMiddleware<LoggingContextMiddleware>()
            .UseMiddleware<SentryContextMiddleware>()
            .UseMiddleware<RequestLoggingMiddleware>() // 直接写中间件本体即可
            .UseMiddleware<ExceptionHandlingMiddleware>();
    }
}