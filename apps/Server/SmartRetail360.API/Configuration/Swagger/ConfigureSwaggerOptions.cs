using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace SmartRetail360.API.Configuration.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var desc in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(desc.GroupName, new OpenApiInfo
            {
                Title = $"SmartRetail360 API {desc.ApiVersion}",
                Version = desc.ApiVersion.ToString(),
                Description = "API documentation for SmartRetail360 platform",
                TermsOfService = new Uri("https://smartretail360.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "SmartRetail360 Team",
                    Email = "support@smartretail360.com",
                    Url = new Uri("https://smartretail360.com")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
        }
    }
}