using System.Globalization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using SmartRetail360.API.Extensions;
using SmartRetail360.Application;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Infrastructure;
using SmartRetail360.Infrastructure.Logging.Context;

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

        // Register Api, Application, and Infrastructure layers
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(Configuration);
        services.AddApiLayer(Configuration);
        
        // Http Context Accessor(Should be in Startup.cs)
        services.AddHttpContextAccessor();
        services.AddScoped<ILogContextAccessor, LogContextAccessor>();
    }

    // Configure middleware pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Localization(Should be before exception handling middleware)
        var locOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(locOptions.Value);

        // Read X-Locale header and set culture
        app.Use(async (context, next) =>
        {
            if (context.Request.Headers.TryGetValue("X-Locale", out var cultureValues))
            {
                var culture = cultureValues.ToString();
                if (!string.IsNullOrEmpty(culture))
                {
                    var cultureInfo = new CultureInfo(culture);
                    CultureInfo.CurrentCulture = cultureInfo;
                    CultureInfo.CurrentUICulture = cultureInfo;
                }
            }

            await next();
        });

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
                    options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
                }
            });
        }
        
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}