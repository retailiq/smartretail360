using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Application.Models;
using SmartRetail360.Application.Validators.AccountRegistration;
using SmartRetail360.Shared.Localization;

namespace SmartRetail360.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<AccountRegisterRequestValidator>();
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        
        services.AddScoped<ApplicationDependencies>(sp => new ApplicationDependencies
        {
            Localizer = sp.GetRequiredService<MessageLocalizer>(),
        });

        return services;
    }
}