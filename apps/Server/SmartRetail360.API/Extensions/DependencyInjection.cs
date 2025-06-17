using Microsoft.OpenApi.Models;
using SmartRetail360.API.Configuration.Swagger;
using SmartRetail360.Shared.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration config)
    {
        // Controller
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

        services.AddEndpointsApiExplorer();
        
        // Application Options
        services.Configure<AppOptions>(config.GetSection(GeneralConstants.App));
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AppOptions>>().Value);

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
                Name = GeneralConstants.Authorization,
                Type = SecuritySchemeType.ApiKey,
                Scheme = GeneralConstants.Bearer,
                BearerFormat = GeneralConstants.Jwt,
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