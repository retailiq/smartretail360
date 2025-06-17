using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.ABAC.Configs;
using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.ABAC.Services.Resolvers;
using SmartRetail360.ABAC.Services;
using SmartRetail360.ABAC.Services.AbacPolicyService;

namespace SmartRetail360.ABAC;

public static class DependencyInjection
{
    public static IServiceCollection AddAbac(this IServiceCollection services, IConfiguration configuration)
    
    {
        services.Scan(scan => scan
            .FromAssemblyOf<UserResourceResolver>()
            .AddClasses(classes => classes.AssignableTo<ICustomResourceResolver>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddScoped<IPolicyRepo, PolicyRepo>();
        services.AddScoped<IPolicyEvaluator, JsonLogicPolicyEvaluator>();
        services.AddScoped<IResourceAttributeResolver, CompositeResourceAttributeResolver>();
        services.AddScoped<IAbacPolicyService, AbacPolicyService>();
        services.AddScoped<IAbacPolicyGetAllService, AbacPolicyGetAllService>();
        services.AddScoped<IAbacPolicyUpdateService, AbacPolicyUpdateService>();
        services.AddScoped<IAbacPolicySyncService, AbacPolicySyncService>();
        services.AddScoped<IAbacPolicyTemplateCreateService, AbacPolicyTemplateCreateService>();
        services.AddScoped<IAbacPolicyApplyTemplateService, AbacPolicyApplyTemplateService>();
        services.AddScoped<IAbacPolicyDefaultCreateService, AbacPolicyDefaultCreateService>();
        services.AddScoped<IAbacPolicyDeriveService, AbacPolicyDeriveService>();

        var enabled = configuration.GetValue<bool>("Abac:EnableRouteAuthorization");
        if (enabled)
        {
            var routeConfigPath = Path.Combine(AppContext.BaseDirectory, "Configs", "abac.routes.json");
            if (!File.Exists(routeConfigPath))
                throw new FileNotFoundException($"ABAC route config file not found: {routeConfigPath}");
            var jsonText = File.ReadAllText(routeConfigPath);
            var rules = JsonSerializer.Deserialize<List<AbacRouteRule>>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (rules == null || rules.Count == 0)
                throw new InvalidOperationException("ABAC route config file is empty or invalid.");
            services.AddSingleton(new AbacRouteMapper(rules));
        }

        return services;
    }
}