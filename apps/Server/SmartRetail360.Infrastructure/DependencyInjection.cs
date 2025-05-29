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
using SmartRetail360.Infrastructure.Services.Notifications;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Strategies;
using SmartRetail360.Infrastructure.Services.Notifications.Templates;
using StackExchange.Redis;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Interfaces.Caching;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Application.Interfaces.Messaging;
using SmartRetail360.Application.Interfaces.Redis;
using SmartRetail360.Infrastructure.Common.DependencyInjection;
using SmartRetail360.Infrastructure.Common.Execution;
using SmartRetail360.Infrastructure.Logging;
using SmartRetail360.Infrastructure.Logging.Dispatcher;
using SmartRetail360.Infrastructure.Logging.Loggers;
using SmartRetail360.Infrastructure.Logging.Policies;
using SmartRetail360.Infrastructure.Services.AccountRegistration.Models;
using SmartRetail360.Infrastructure.Services.Auth;
using SmartRetail360.Infrastructure.Services.Auth.AccountActivationEmailVerification;
using SmartRetail360.Infrastructure.Services.Auth.Login.CredentialsLogin;
using SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin;
using SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Handlers;
using SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Options;
using SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Strategies;
using SmartRetail360.Infrastructure.Services.Auth.Login.TenantLogin;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Infrastructure.Services.Auth.Tokens;
using SmartRetail360.Infrastructure.Services.Common;
using SmartRetail360.Infrastructure.Services.Messaging;
using SmartRetail360.Infrastructure.Services.Notifications.Models;
using SmartRetail360.Infrastructure.Services.Redis;
using SmartRetail360.Shared.Constants;

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

        // HttpClient Configuration
        services.AddHttpClient(GeneralConstants.GoogleOAuth);
        services.AddHttpClient(GeneralConstants.FacebookOAuth);
        services.AddHttpClient(GeneralConstants.MicrosoftOAuth);

        services.Configure<OAuthOptions>(config.GetSection("OAuth"));

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
        services.AddScoped<ILoginService, CredentialsLoginService>();
        services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IOAuthLoginService, OAuthLoginService>();
        services.AddScoped<IConfirmTenantLoginService, ConfirmTenantLoginService>();
        services.AddScoped<GoogleOAuthHandler>();
        services.AddScoped<OAuthProviderStrategy>();
        services.AddScoped<FacebookOAuthHandler>();
        services.AddScoped<MicrosoftOAuthHandler>();
        services.AddScoped<IAuthService, AuthService>();

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


        services.AddScoped<AccountRegistrationDependencies>(sp =>
            DependencyBuilder.Build<AccountRegistrationDependencies>(sp, deps =>
            {
                deps.EmailContext = sp.GetRequiredService<EmailContext>();
                deps.AuditLogger = sp.GetRequiredService<IAuditLogger>();
                deps.EmailQueueProducer = sp.GetRequiredService<IEmailQueueProducer>();
            }));
        services.AddScoped<AccountActivationEmailVerificationDependencies>(sp =>
            DependencyBuilder.Build<AccountActivationEmailVerificationDependencies>(sp, deps => { }));
        services.AddScoped<AuthTokenDependencies>(sp =>
            DependencyBuilder.Build<AuthTokenDependencies>(sp, deps =>
            {
                deps.AccessTokenGenerator = sp.GetRequiredService<IAccessTokenGenerator>();
                deps.RefreshTokenService = sp.GetRequiredService<IRefreshTokenService>();
                deps.HttpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!;
            }));
        services.AddScoped<LoginDependencies>(sp =>
            DependencyBuilder.Build<LoginDependencies>(sp, deps =>
            {
                deps.AccessTokenGenerator = sp.GetRequiredService<IAccessTokenGenerator>();
                deps.AccountSupport = sp.GetRequiredService<IAccountSupportService>();
            }));
        services.AddScoped<OAuthLoginDependencies>(sp =>
            DependencyBuilder.Build<OAuthLoginDependencies>(sp, deps =>
            {
                deps.AccessTokenGenerator = sp.GetRequiredService<IAccessTokenGenerator>();
                deps.AccountSupport = sp.GetRequiredService<IAccountSupportService>();
                deps.OAuthProviderStrategy = sp.GetRequiredService<OAuthProviderStrategy>();
            }));
        services.AddScoped<ConfirmTenantLoginDependencies>(sp =>
            DependencyBuilder.Build<ConfirmTenantLoginDependencies>(sp, deps =>
            {
                deps.AccessTokenGenerator = sp.GetRequiredService<IAccessTokenGenerator>();
                deps.RefreshTokenService = sp.GetRequiredService<IRefreshTokenService>();
                deps.HttpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!;
            }));
        services.AddScoped<NotificationDependencies>(sp =>
            DependencyBuilder.Build<NotificationDependencies>(sp, deps =>
            {
                deps.EmailQueueProducer = sp.GetRequiredService<IEmailQueueProducer>();
                deps.RedisLimiterService = sp.GetRequiredService<IRedisLimiterService>();
                deps.AccountSupport = sp.GetRequiredService<IAccountSupportService>();
            }));

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

        services.AddSingleton<IEmailQueueProducer, SqsEmailProducer>();
        services.AddScoped<ISafeExecutor, SafeExecutor>();
        services.AddTransient<IGuardChecker, GuardChecker>();
        services.AddScoped<IPlatformContextService, PlatformContextService>();
        services.AddScoped<IAccountSupportService, AccountSupportService>();

        return services;
    }
}