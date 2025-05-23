using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.API.Middlewares;

public class SentryContextMiddleware
{
    private readonly RequestDelegate _next;

    public SentryContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogContextAccessor logContext)
    {
        SentrySdk.ConfigureScope(scope =>
        {
            // ✅ User
            if (logContext.UserId is Guid uid)
            {
                scope.User = new SentryUser
                {
                    Id = uid.ToString(),
                    Email = logContext.Email
                };
            }

            // ✅ Tags
            scope.SetTag("TraceId", logContext.TraceId ?? GeneralConstants.Unknown);
            scope.SetTag("Email", logContext.Email ?? GeneralConstants.Unknown);
            scope.SetTag("Locale", logContext.Locale ?? GeneralConstants.Unknown);
            scope.SetTag("UserId", logContext.UserId?.ToString() ?? GeneralConstants.Unknown);
            scope.SetTag("TenantId", logContext.TenantId?.ToString() ?? GeneralConstants.Unknown);
            scope.SetTag("RoleId", logContext.RoleId?.ToString() ?? GeneralConstants.Unknown);
            scope.SetTag("Module", logContext.Module ?? GeneralConstants.Unknown);
            scope.SetTag("IpAddress", logContext.IpAddress ?? GeneralConstants.Unknown);
            scope.SetTag("RoleName", logContext.RoleName ?? GeneralConstants.Unknown);
            scope.SetTag("Action", logContext.Action ?? GeneralConstants.Unknown);
            scope.SetTag("LogId", logContext.LogId ?? GeneralConstants.Unknown);
        });

        await _next(context);
    }
}