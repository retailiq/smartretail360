using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Interfaces.Auth.Configuration;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Notifications.Configuration;
using SmartRetail360.Application.Interfaces.Notifications.Strategies;
using SmartRetail360.Application.Interfaces.Services;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Interceptors;
using SmartRetail360.Infrastructure.Services.AccountRegistration;
using SmartRetail360.Infrastructure.Services.Auth;
using SmartRetail360.Infrastructure.Services.Auth.Configuration;
using SmartRetail360.Infrastructure.Services.Common;
using SmartRetail360.Infrastructure.Services.Notifications;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Strategies;
using SmartRetail360.Infrastructure.Services.Notifications.Templates;
using StackExchange.Redis;
using Scrutor;
using SmartRetail360.Application.Common;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Infrastructure.Common.DependencyInjection;
using SmartRetail360.Infrastructure.Common.Execution;
using SmartRetail360.Infrastructure.Logging;
using SmartRetail360.Infrastructure.Logging.Dispatcher;
using SmartRetail360.Infrastructure.Logging.Loggers;
using SmartRetail360.Infrastructure.Logging.Policies;
using SmartRetail360.Infrastructure.Services.AccountRegistration.Models;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Infrastructure.Services.Messaging;
using SmartRetail360.Infrastructure.Services.Notifications.Models;
using SmartRetail360.Infrastructure.Services.Redis;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped(typeof(Lazy<>), typeof(LazyResolver<>));
        
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

        // Redis Service
        // Redis Configuration
        var redis = ConnectionMultiplexer.Connect(config["Redis:ConnectionString"]);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        // Redis Limiter
        services.AddScoped<IRedisLimiterService, RedisRedisLimiterService>();
        // Redis Lock
        services.AddScoped<IRedisLockService, RedisRedisLockService>();
        // Redis Log Sampling
        services.AddScoped<IRedisLogSamplingService, RedisLogSamplingLogSamplingService>();

        // Register the Tenant Registration Dependencies
        services.AddScoped<TenantRegistrationDependencies>(sp => new TenantRegistrationDependencies
        {
            Db = sp.GetRequiredService<AppDbContext>(),
            UserContext = sp.GetRequiredService<IUserContextService>(),
            Localizer = sp.GetRequiredService<MessageLocalizer>(),
            EmailContext = sp.GetRequiredService<EmailContext>(),
            RedisLockService = sp.GetRequiredService<IRedisLockService>(),
            AppOptions = sp.GetRequiredService<AppOptions>(),
            AuditLogger = sp.GetRequiredService<IAuditLogger>(),
            LogDispatcher = sp.GetRequiredService<ILogDispatcher>(),
            EmailQueueProducer = sp.GetRequiredService<SqsEmailProducer>(),
            SafeExecutor = sp.GetRequiredService<ISafeExecutor>(),
            GuardChecker = sp.GetRequiredService<IGuardChecker>()
        });
        
        // Register the Auth Dependencies
        services.AddScoped<AuthDependencies>(sp => new AuthDependencies
        {
            Db = sp.GetRequiredService<AppDbContext>(),
            UserContext = sp.GetRequiredService<IUserContextService>(),
            Localizer = sp.GetRequiredService<MessageLocalizer>(),
            AppOptions = sp.GetRequiredService<AppOptions>(),
            LogDispatcher = sp.GetRequiredService<ILogDispatcher>(),
            SafeExecutor = sp.GetRequiredService<ISafeExecutor>(),
            RedisLimiterService = sp.GetRequiredService<IRedisLimiterService>(),
            GuardChecker = sp.GetRequiredService<IGuardChecker>()
        });

        // Register the Notification Dependencies
        services.AddScoped<NotificationDependencies>(sp => new NotificationDependencies
        {
            Db = sp.GetRequiredService<AppDbContext>(),
            AppOptions = sp.GetRequiredService<AppOptions>(),
            UserContext = sp.GetRequiredService<IUserContextService>(),
            Localizer = sp.GetRequiredService<MessageLocalizer>(),
            RedisLimiterService = sp.GetRequiredService<IRedisLimiterService>(),
            LogDispatcher = sp.GetRequiredService<ILogDispatcher>(),
            EmailQueueProducer = sp.GetRequiredService<SqsEmailProducer>(),
            SafeExecutor = sp.GetRequiredService<ISafeExecutor>(),
            GuardChecker = sp.GetRequiredService<IGuardChecker>()
        });

        services.AddScoped<ILogDispatcher, LogDispatcher>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        
        services.AddSingleton<ILogWritePolicyProvider, DefaultLogWritePolicyProvider>();
        services.AddScoped<ILogWriter, DefaultLogWriter>();

        services.AddSingleton<SqsEmailProducer>();

        services.AddSingleton<IAmazonSQS>(sp =>
        {
            return new AmazonSQSClient(
                config["AWS:AccessKey"],
                config["AWS:SecretKey"],
                new AmazonSQSConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.APSoutheast2
                });
        });

        services.AddScoped<ISafeExecutor, SafeExecutor>();
        
        services.AddTransient<IGuardChecker, GuardChecker>();

        return services;
    }
}