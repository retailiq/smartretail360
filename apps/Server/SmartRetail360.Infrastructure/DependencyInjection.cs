using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Users;
using SmartRetail360.Caching.Interfaces;
using SmartRetail360.Infrastructure.Services.AccountRegistration;
using SmartRetail360.Infrastructure.Common.DependencyInjection;
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
using SmartRetail360.Infrastructure.Services.Notifications;
using SmartRetail360.Infrastructure.Services.Notifications.Models;
using SmartRetail360.Infrastructure.Services.Users;
using SmartRetail360.Infrastructure.Services.Users.Models;
using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Messaging.Interfaces;
using SmartRetail360.Notifications.Services.Configuration;
using SmartRetail360.Platform.Interfaces;
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration config)
    {
        // HttpClient Configuration
        services.AddHttpClient(GeneralConstants.GoogleOAuth);
        services.AddHttpClient(GeneralConstants.FacebookOAuth);
        services.AddHttpClient(GeneralConstants.MicrosoftOAuth);

        services.Configure<OAuthOptions>(config.GetSection(GeneralConstants.OAuth));

        // Users Related Services
        services.AddScoped<IUserProfileUpdateService, UserProfileUpdateService>();
        services.AddScoped<UpdateUserProfileTokenGenerator>();
        
        // Email Related Services
        services.AddScoped<IAccountEmailVerificationService, AccountActivationEmailVerificationService>();
        services.AddScoped<IAccountActivationEmailResendingService, AccountActivationEmailResendingService>();
        services.AddScoped<IEmailDispatchService, EmailDispatchService>();

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

        services.AddScoped<AccountRegistrationDependencies>(sp =>
            DependencyBuilder.Build<AccountRegistrationDependencies>(sp, deps =>
            {
                deps.EmailContext = sp.GetRequiredService<EmailContext>();
                deps.AuditLogger = sp.GetRequiredService<IAuditLogger>();
                deps.EmailQueueProducer = sp.GetRequiredService<IEmailQueueProducer>();
                deps.AbacPolicyService = sp.GetRequiredService<IAbacPolicyService>();
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
        services.AddScoped<UsersDependencies>(sp =>
            DependencyBuilder.Build<UsersDependencies>(sp, deps =>
            {
                deps.UpdateUserProfileTokenGenerator = sp.GetRequiredService<UpdateUserProfileTokenGenerator>();
                deps.HttpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!;
            }));
        services.AddScoped<LoginDependencies>(sp =>
            DependencyBuilder.Build<LoginDependencies>(sp, deps =>
            {
                deps.AccessTokenGenerator = sp.GetRequiredService<IAccessTokenGenerator>();
                deps.AccountSupport = sp.GetRequiredService<IAccountSupportService>();
                deps.AbacPolicyService = sp.GetRequiredService<IAbacPolicyService>();
            }));
        services.AddScoped<OAuthLoginDependencies>(sp =>
            DependencyBuilder.Build<OAuthLoginDependencies>(sp, deps =>
            {
                deps.AccessTokenGenerator = sp.GetRequiredService<IAccessTokenGenerator>();
                deps.AccountSupport = sp.GetRequiredService<IAccountSupportService>();
                deps.OAuthProviderStrategy = sp.GetRequiredService<OAuthProviderStrategy>();
                deps.AbacPolicyService = sp.GetRequiredService<IAbacPolicyService>();
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

        return services;
    }
}