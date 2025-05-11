using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SmartRetail360.API.Configuration.Swagger;
using SmartRetail360.API.Middlewares;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;
using System.Globalization;

namespace SmartRetail360.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration config)
    {
        // Controller
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        services.AddEndpointsApiExplorer();

        // App 配置
        services.Configure<AppOptions>(config.GetSection("App"));
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AppOptions>>().Value);
        
        // Register MultiLanguage Resources
        services.AddLocalization(options => { options.ResourcesPath = "Localization"; });
        
        // 国际化
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

        return services;
    }
}