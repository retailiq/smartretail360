using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SmartRetail360.ABAC;
using SmartRetail360.API.Extensions;
using SmartRetail360.Application;
using SmartRetail360.Execution;
using SmartRetail360.Infrastructure;
using SmartRetail360.Logging;
using SmartRetail360.Messaging;
using SmartRetail360.Notifications;
using SmartRetail360.Persistence;
using SmartRetail360.Platform;
using SmartRetail360.Shared;
using SmartRetail360.Shared.Contexts;
using SmartRetail360.Shared.Localization;
using DependencyInjection = SmartRetail360.Logging.DependencyInjection;

namespace SmartRetail360.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // Register services dependencies
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthorization();

        // Register Layers and Shared Contexts
        DependencyInjection.AddLogging(services
            .AddApplicationLayer()
            .AddInfrastructureLayer(Configuration)
            .AddApiLayer(Configuration)
            .AddPersistence(Configuration)
            .AddMessaging(Configuration)
            .AddExecution()
            .AddPlatform()
            .AddAbac(Configuration)
            .AddNotifications()
            .AddSharedContexts()
            .AddCaching(Configuration)
            .AddShared(Configuration));

        // Register SmartRetail Localization
        services.AddSmartRetailLocalization(Configuration);

        // Http Context Accessor(Should be in Startup.cs)
        services.AddHttpContextAccessor();
    }

    // Configure middleware pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //Localization Middleware
        app.UseSmartRetailLocalization();
        // Customized Middleware
        app.UseSmartRetailMiddlewares();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var desc in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
                        desc.GroupName.ToUpperInvariant());
                }
            });
        }

        // app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}