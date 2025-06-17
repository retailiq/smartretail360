using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Sentry.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Logging.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseSmartRetailSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, loggerConfig) =>
        {
            loggerConfig
                .MinimumLevel.Debug()
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithSpan(); // Adds trace/span ID for Grafana/Tempo
        });
    }
    
    public static IServiceCollection AddSmartRetailTelemetry(this IServiceCollection services, IConfiguration config, string serviceName = GeneralConstants.Sr360DotApi)
    {
        var otlpEndpoint = config.GetValue<string>("OpenTelemetry:Otlp:Endpoint") ?? "http://localhost:4317";

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .SetSampler(new TraceIdRatioBasedSampler(0.2)) // Sample 20% of traces
                    .AddSource(serviceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(otlpEndpoint);
                        opt.Protocol = OtlpExportProtocol.Grpc;
                    });
            });

        return services;
    }
    
    public static void AddSmartRetailSentry(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseSentry((WebHostBuilderContext context, SentryAspNetCoreOptions options) =>
        {
            context.Configuration.GetSection(GeneralConstants.Sentry).Bind(options);
        });
    }
}