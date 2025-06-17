using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using SmartRetail360.Shared.Logging;
using SmartRetail360.Shared.Options;
using LogContext = Serilog.Context.LogContext;

namespace SmartRetail360.Gateway.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppOptions _appOptions;

    public RequestLoggingMiddleware(RequestDelegate next, IOptions<AppOptions> appOptions)
    {
        _next = next;
        _appOptions = appOptions.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        string? requestBody = null;

        try
        {
            var shouldReadBody = _appOptions.EnableRequestBodyLogging &&
                                 context.Request.ContentLength > 0 &&
                                 (context.Request.Method == "POST" || context.Request.Method == "PUT") &&
                                 context.Request.Path.StartsWithSegments("/api") &&
                                 !(context.Request.ContentType?.StartsWith("multipart/form-data",
                                     StringComparison.OrdinalIgnoreCase) ?? false);

            if (shouldReadBody)
            {
                context.Request.EnableBuffering();

                using var reader = new StreamReader(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true
                );

                var rawBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (rawBody.Length > _appOptions.RequestBodyMaxLength)
                {
                    rawBody = rawBody[.._appOptions.RequestBodyMaxLength] + "...(truncated)";
                }

                requestBody = _appOptions.EnableSensitiveFieldMasking
                    ? TrySanitizeJson(rawBody, _appOptions.SensitiveFields)
                    : rawBody;
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            sw.Stop();

            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            using (LogContext.PushProperty("StatusCategory", GetStatusCategory(500)))
            using (LogContext.PushProperty("LogCategory", LogCategory.Application))
            {
                Log.Error(ex,
                    "🔥 {Method} {Path} failed in {Elapsed}ms | Body: {RequestBody}",
                    context.Request.Method,
                    context.Request.Path,
                    sw.ElapsedMilliseconds,
                    requestBody ?? "[no body]"
                );
            }

            throw new InvalidOperationException(
                $"Exception occurred while processing request at {context.Request.Path}", ex);
        }

        sw.Stop();

        var elapsed = sw.ElapsedMilliseconds;
        var level = elapsed > _appOptions.RequestTimeoutThresholdMs ? LogEventLevel.Warning : LogEventLevel.Information;

        var statusCode = context.Response.StatusCode;

        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        using (LogContext.PushProperty("StatusCategory", GetStatusCategory(statusCode)))
        using (LogContext.PushProperty("LogCategory", LogCategory.Application))
        {
            Log.Write(level,
                "📘 {Method} {Path} responded {StatusCode} in {Elapsed}ms | Body: {RequestBody}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                elapsed,
                requestBody ?? "[no body]"
            );
        }
    }

    private static string GetStatusCategory(int statusCode)
    {
        var prefix = statusCode / 100;
        return $"{prefix}xx";
    }

    private static string TrySanitizeJson(string raw, List<string> sensitiveFields)
    {
        try
        {
            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(raw);

            if (json == null) return raw;

            foreach (var key in json.Keys
                         .Where(k => sensitiveFields.Any(f => k.Contains(f, StringComparison.OrdinalIgnoreCase)))
                         .ToList())
            {
                json[key] = "***";
            }

            return JsonSerializer.Serialize(json);
        }
        catch
        {
            return raw;
        }
    }
}