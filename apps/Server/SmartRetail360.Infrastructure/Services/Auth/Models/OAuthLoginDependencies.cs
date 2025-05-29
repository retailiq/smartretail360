using SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Strategies;

namespace SmartRetail360.Infrastructure.Services.Auth.Models;

public class OAuthLoginDependencies : LoginDependencies
{
    public OAuthProviderStrategy OAuthProviderStrategy { get; set; }
}