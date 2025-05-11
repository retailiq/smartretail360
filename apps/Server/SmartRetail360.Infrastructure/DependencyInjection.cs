using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Notifications.Configuration;
using SmartRetail360.Application.Interfaces.TenantManagement;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Interceptors;
using SmartRetail360.Infrastructure.Services.Auth;
using SmartRetail360.Infrastructure.Services.Notifications;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Strategies;
using SmartRetail360.Infrastructure.Services.Notifications.Templates;
using SmartRetail360.Infrastructure.Services.TenantManagement;

namespace SmartRetail360.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration config)
    {
        // 注入 EF Core 及拦截器
        services.AddSingleton<EntityTimestampsInterceptor>();
        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            var interceptor = provider.GetRequiredService<EntityTimestampsInterceptor>();
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(interceptor);
        });

        // 邮件相关服务
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        services.AddScoped<IEmailVerificationService, EmailVerificationService>();
        services.AddScoped<IAccountActivateEmailResendingService, AccountActivateEmailResendingService>();
        services.AddScoped<IEmailSender, MailKitEmailSender>();
        services.AddScoped<IEmailTemplateProvider, DefaultEmailTemplateProvider>();
        services.AddScoped<AccountActivationTemplate>();
        services.AddScoped<IEmailDispatchService, EmailDispatchService>();
        services.AddScoped<EmailContext>();
        services.AddScoped<AccountActivationEmailStrategy>();
        // services.AddScoped<VerificationCodeEmailStrategy>();

        // 注册租户服务
        services.AddScoped<ITenantRegistrationService, TenantRegistrationService>();

        return services;
    }
}