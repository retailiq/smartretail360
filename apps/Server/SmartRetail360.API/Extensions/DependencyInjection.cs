using Microsoft.AspNetCore.Localization;
using Microsoft.OpenApi.Models;
using SmartRetail360.API.Configuration.Swagger;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;
using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SmartRetail360.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration config)
    {
        // Controller
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        services.AddEndpointsApiExplorer();

        // Application Options
        services.Configure<AppOptions>(config.GetSection("App"));
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AppOptions>>().Value);

        // Register MultiLanguage Resources
        services.AddLocalization(options => { options.ResourcesPath = "Localization"; });

        // Internationalization
        var appOptions = config.GetSection("App").Get<AppOptions>();
        var supportedCultures = appOptions.SupportedCultures?.Any() == true
            ? appOptions.SupportedCultures
            : new List<string> { "en", "zh-CN" }; // fallback

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(appOptions.DefaultCulture ?? "en");
            options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
            options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
        });

        services.AddScoped<MessageLocalizer>();

        // API Versioning
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader()
            );
        });

        // Swagger API 
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Swagger
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter: **Bearer {your JWT token}**"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.ConfigureOptions<ConfigureSwaggerOptions>();

        // otlp configuration
        var otlpEndpoint = config.GetValue<string>("OpenTelemetry:Otlp:Endpoint")
                           ?? "http://localhost:4317";
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("SmartRetail360.API"))
            .WithTracing(tracing =>
            {
                tracing
                    .SetSampler(new TraceIdRatioBasedSampler(0.2)) // ✅ Sample 20% of traces
                    .AddSource("SmartRetail360.API")
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(otlpEndpoint);
                        opt.Protocol = OtlpExportProtocol.Grpc;
                    });
            });


        return services;
    }
}