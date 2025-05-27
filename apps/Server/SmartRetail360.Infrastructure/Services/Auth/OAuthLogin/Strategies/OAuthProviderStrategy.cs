using Microsoft.Extensions.DependencyInjection;
using SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Handlers;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Strategies;

public class OAuthProviderStrategy
{
    private readonly IServiceProvider _serviceProvider;

    public OAuthProviderStrategy(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOAuthProviderHandler? Resolve(OAuthProvider provider)
    {
        return provider switch
        {
            OAuthProvider.Google => _serviceProvider.GetService<GoogleOAuthHandler>(),
            OAuthProvider.Facebook => _serviceProvider.GetService<FacebookOAuthHandler>(),
            OAuthProvider.Microsoft => _serviceProvider.GetService<MicrosoftOAuthHandler>(),
            _ => null
        };
    }
}