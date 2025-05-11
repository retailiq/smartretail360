using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Application.Common;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Validators.Auth;

namespace SmartRetail360.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // 注入 UserContext
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IUserContextService, UserContextService>();

        // FluentValidation 注册
        services.AddValidatorsFromAssemblyContaining<TenantRegisterRequestValidator>();
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        return services;
    }
}