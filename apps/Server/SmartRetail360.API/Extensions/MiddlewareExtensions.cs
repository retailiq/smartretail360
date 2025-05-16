using SmartRetail360.API.Middlewares;

namespace SmartRetail360.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}