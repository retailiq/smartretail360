using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Interfaces.Auth.Configuration;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Notifications.Configuration;
using SmartRetail360.Application.Interfaces.Notifications.Strategies;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Interceptors;
using SmartRetail360.Infrastructure.Services.AccountRegistration;
using SmartRetail360.Infrastructure.Services.Auth;
using SmartRetail360.Infrastructure.Services.Auth.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Strategies;
using SmartRetail360.Infrastructure.Services.Notifications.Templates;

namespace SmartRetail360.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration config)
    {
        // Inject the Interceptors
        services.AddSingleton<EntityTimestampsInterceptor>();
        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            var interceptor = provider.GetRequiredService<EntityTimestampsInterceptor>();
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(interceptor);
        });

        // Email Related Services
        services.AddScoped<IEmailVerificationService, TenantAccountEmailVerificationService>();
        services.AddScoped<IAccountActivateEmailResendingService, TenantAccountActivateEmailResendingService>();
        services.AddScoped<IEmailSender, MailKitEmailSender>();
        services.AddScoped<IEmailTemplateProvider, DefaultEmailTemplateProvider>();
        services.AddScoped<AccountActivationTemplate>();
        services.AddScoped<IEmailDispatchService, EmailDispatchService>();
        services.AddScoped<EmailContext>();
        services.AddScoped<TenantAccountActivationEmailStrategy>();
        services.AddScoped<IEmailStrategy, TenantAccountActivationEmailStrategy>();

        // Register the Tenant Registration Service
        services.AddScoped<ITenantRegistrationService, TenantRegistrationService>();
        
        // Register Email Verification
        services.AddScoped<IEmailVerificationDispatchService, EmailVerificationDispatchService>();
        services.AddScoped<IEmailVerificationService, TenantAccountEmailVerificationService>();
        
        return services;
    }
}