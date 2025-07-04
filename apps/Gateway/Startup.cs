using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SmartRetail360.ABAC;
using SmartRetail360.Execution;
using SmartRetail360.Gateway.Middlewares;
using SmartRetail360.Messaging;
using SmartRetail360.Persistence;
using SmartRetail360.Platform;
using SmartRetail360.Shared;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;
using DependencyInjection = SmartRetail360.Logging.DependencyInjection;

namespace SmartRetail360.Gateway;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        services.AddControllers().AddJsonOptions(options =>
        {
            // Ignore null values globally
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull;
            // Use camelCase and allow integer values for enums
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true)
            );
            
            // Ignore case for property names
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

        services.Configure<AppOptions>(Configuration.GetSection(GeneralConstants.App));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppOptions>>().Value);

        services.AddHttpContextAccessor();
        services.AddReverseProxy()
            .LoadFromConfig(Configuration.GetSection("ReverseProxy"));

        // Register SmartRetail Localization
        services.AddSmartRetailLocalization(Configuration);

        DependencyInjection.AddCustomLogging(services)
            .AddPersistence(Configuration)
            .AddAbac(Configuration)
            .AddPlatform()
            .AddShared(Configuration)
            .AddExecution()
            .AddCaching(Configuration)
            .AddMessaging(Configuration)
            .AddSharedContexts();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSmartRetailLocalization();
        app.UseMiddleware<RequestContextEnrichmentMiddleware>();
        app.UseMiddleware<LoggingContextMiddleware>();
        app.UseMiddleware<JwtValidationMiddleware>();
        app.UseMiddleware<SentryContextMiddleware>();
        app.UseMiddleware<AbacAuthorizationMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseRouting();
        
        app.UseCors("AllowFrontend");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapReverseProxy();
        });
    }
}