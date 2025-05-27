using Amazon.SQS;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Notifications.Configuration;
using SmartRetail360.Application.Interfaces.Notifications.Strategies;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Interceptors;
using SmartRetail360.Infrastructure.Services.AccountRegistration;
using SmartRetail360.Infrastructure.Services.Auth;
using SmartRetail360.Infrastructure.Services.Notifications;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Strategies;
using SmartRetail360.Infrastructure.Services.Notifications.Templates;
using StackExchange.Redis;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Caching;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Application.Interfaces.Redis;
using SmartRetail360.Infrastructure.Common.DependencyInjection;
using SmartRetail360.Infrastructure.Common.Execution;
using SmartRetail360.Infrastructure.Logging;
using SmartRetail360.Infrastructure.Logging.Dispatcher;
using SmartRetail360.Infrastructure.Logging.Loggers;
using SmartRetail360.Infrastructure.Logging.Policies;
using SmartRetail360.Infrastructure.Services.AccountRegistration.Models;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Infrastructure.Services.Auth.OAuthLogin;
using SmartRetail360.Infrastructure.Services.Auth.Tokens;
using SmartRetail360.Infrastructure.Services.Auth.UserLogin;
using SmartRetail360.Infrastructure.Services.Common;
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
        services.AddScoped<IAccountEmailVerificationService, AccountActivationEmailVerificationService>();
        services.AddScoped<IAccountActivationEmailResendingService, AccountActivationEmailResendingService>();
        services.AddScoped<IEmailSender, MailKitEmailSender>();
        services.AddScoped<IEmailTemplateProvider, DefaultEmailTemplateProvider>();
        services.AddScoped<AccountRegistrationActivationTemplate>();
        services.AddScoped<IEmailDispatchService, EmailDispatchService>();
        services.AddScoped<EmailContext>();
        services.AddScoped<AccountRegistrationActivationEmailStrategy>();
        services.AddScoped<IEmailStrategy, AccountRegistrationActivationEmailStrategy>();

        // Register the Tenant Registration Service
        services.AddScoped<IAccountRegistrationService, AccountRegistrationService>();

        // Auth Related Services
        services.AddScoped<IAccountEmailVerificationService, AccountActivationEmailVerificationService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
        services.AddScoped<IConfirmTenantLoginService, ConfirmTenantLoginService1>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IOAuthLoginService, OAuthLoginService>();
        
        // Redis Service
        var redis = ConnectionMultiplexer.Connect(config["Redis:ConnectionString"]!);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        services.AddScoped<IRedisLimiterService, RedisRedisLimiterService>();
        services.AddScoped<IRedisLockService, RedisRedisLockService>();
        services.AddScoped<IRedisLogSamplingService, RedisLogSamplingService>();
        services.AddScoped<IRoleCacheService, RoleCacheService>();
        services.AddScoped<IActivationTokenCacheService, ActivationTokenCacheService>();
        services.AddScoped<IRedisOperationService, RedisOperationService>();
        services.AddScoped<ILoginFailureLimiter, LoginFailureLimiter>();

        // Register the Tenant Registration Dependencies
        services.AddScoped<AccountRegistrationDependencies>(sp => new AccountRegistrationDependencies
        {
            Db = sp.GetRequiredService<AppDbContext>(),
            UserContext = sp.GetRequiredService<IUserContextService>(),
            Localizer = sp.GetRequiredService<MessageLocalizer>(),
            EmailContext = sp.GetRequiredService<EmailContext>(),
            AppOptions = sp.GetRequiredService<AppOptions>(),
            AuditLogger = sp.GetRequiredService<IAuditLogger>(),
            LogDispatcher = sp.GetRequiredService<ILogDispatcher>(),
            EmailQueueProducer = sp.GetRequiredService<SqsEmailProducer>(),
            SafeExecutor = sp.GetRequiredService<ISafeExecutor>(),
            GuardChecker = sp.GetRequiredService<IGuardChecker>(),
            RedisOperation = sp.GetRequiredService<IRedisOperationService>(),
            PlatformContext = sp.GetRequiredService<IPlatformContextService>()
        });

        // Register the Email Verification Dependencies
        services.AddScoped<AccountActivationEmailVerificationDependencies>(sp =>
            new AccountActivationEmailVerificationDependencies
            {
                Db = sp.GetRequiredService<AppDbContext>(),
                UserContext = sp.GetRequiredService<IUserContextService>(),
                Localizer = sp.GetRequiredService<MessageLocalizer>(),
                AppOptions = sp.GetRequiredService<AppOptions>(),
                LogDispatcher = sp.GetRequiredService<ILogDispatcher>(),
                SafeExecutor = sp.GetRequiredService<ISafeExecutor>(),
                GuardChecker = sp.GetRequiredService<IGuardChecker>(),
                RedisOperation = sp.GetRequiredService<IRedisOperationService>(),
                PlatformContext = sp.GetRequiredService<IPlatformContextService>()
            });

        // Register the Login Dependencies
        services.AddScoped<LoginDependencies>(sp => new LoginDependencies
        {
            PlatformContext = sp.GetRequiredService<IPlatformContextService>(),
            Localizer = sp.GetRequiredService<MessageLocalizer>(),
            SafeExecutor = sp.GetRequiredService<ISafeExecutor>(),
            GuardChecker = sp.GetRequiredService<IGuardChecker>(),
            RedisOperation = sp.GetRequiredService<IRedisOperationService>(),
            Db = sp.GetRequiredService<AppDbContext>(),
            UserContext = sp.GetRequiredService<IUserContextService>(),
            AppOptions = sp.GetRequiredService<AppOptions>(),
            AccessTokenGenerator = sp.GetRequiredService<IAccessTokenGenerator>(),
            LogDispatcher = sp.GetRequiredService<ILogDispatcher>(),
            AccountSupport = sp.GetRequiredService<IAccountSupportService>()
        });
        
        services.AddScoped<OAuthLoginDependencies>(sp => new OAuthLoginDependencies
        {
            PlatformContext = sp.GetRequiredService<IPlatformContextService>(),
            Localizer = sp.GetRequiredService<MessageLocalizer>(),
            SafeExecutor = sp.GetRequiredService<ISafeExecutor>(),
            GuardChecker = sp.GetRequiredService<IGuardChecker>(),
            RedisOperation = sp.GetRequiredService<IRedisOperationService>(),
            Db = sp.GetRequiredService<AppDbContext>(),
            UserContext = sp.GetRequiredService<IUserContextService>(),
            AppOptions = sp.GetRequiredService<AppOptions>(),
            AccessTokenGenerator = sp.GetRequiredService<IAccessTokenGenerator>(),
            LogDispatcher = sp.GetRequiredService<ILogDispatcher>(),
            AccountSupport = sp.GetRequiredService<IAccountSupportService>()
        });

        services.AddScoped<ConfirmTenantLoginDependencies>(sp => new ConfirmTenantLoginDependencies
        {
            PlatformContext = sp.GetRequiredService<IPlatformContextService>(),
            Localizer = sp.GetRequiredService<MessageLocalizer>(),
            SafeExecutor = sp.GetRequiredService<ISafeExecutor>(),
            GuardChecker = sp.GetRequiredService<IGuardChecker>(),
            RedisOperation = sp.GetRequiredService<IRedisOperationService>(),
            Db = sp.GetRequiredService<AppDbContext>(),
            UserContext = sp.GetRequiredService<IUserContextService>(),
            AppOptions = sp.GetRequiredService<AppOptions>(),
            AccessTokenGenerator = sp.GetRequiredService<IAccessTokenGenerator>(),
            RefreshTokenService = sp.GetRequiredService<IRefreshTokenService>(),
            HttpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!,
            LogDispatcher = sp.GetRequiredService<ILogDispatcher>()
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
            GuardChecker = sp.GetRequiredService<IGuardChecker>(),
            RedisOperation = sp.GetRequiredService<IRedisOperationService>(),
            PlatformContext = sp.GetRequiredService<IPlatformContextService>(),
            AccountSupport = sp.GetRequiredService<IAccountSupportService>()
        });

        // Logs
        services.AddScoped<ILogDispatcher, LogDispatcher>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        services.AddSingleton<ILogWritePolicyProvider, DefaultLogWritePolicyProvider>();
        services.AddScoped<ILogWriter, DefaultLogWriter>();

        // Register the SQS Email Producer
        services.AddSingleton<SqsEmailProducer>();
        services.AddSingleton<IAmazonSQS>(
            new AmazonSQSClient(
                config["AWS:AccessKey"],
                config["AWS:SecretKey"],
                new AmazonSQSConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.APSoutheast2
                }));

        services.AddScoped<ISafeExecutor, SafeExecutor>();
        services.AddTransient<IGuardChecker, GuardChecker>();
        services.AddScoped<IPlatformContextService, PlatformContextService>();
        services.AddScoped<IAccountSupportService, AccountSupportService>();

        return services;
    }
}