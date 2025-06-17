using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Notifications.Interfaces.Configuration;
using SmartRetail360.Notifications.Interfaces.Strategies;
using SmartRetail360.Notifications.Services.Configuration;
using SmartRetail360.Notifications.Services.Strategies;
using SmartRetail360.Notifications.Services.Templates;

namespace SmartRetail360.Notifications;

public static class DependencyInjection
{
    public static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        services.AddScoped<IEmailSender, MailKitEmailSender>();
        services.AddScoped<IEmailTemplateProvider, DefaultEmailTemplateProvider>();
        services.AddScoped<AccountRegistrationActivationTemplate>();

        services.AddScoped<EmailContext>();
        services.AddScoped<AccountRegistrationActivationEmailStrategy>();
        services.AddScoped<IEmailStrategy, AccountRegistrationActivationEmailStrategy>();
        
        return services;
    }
}