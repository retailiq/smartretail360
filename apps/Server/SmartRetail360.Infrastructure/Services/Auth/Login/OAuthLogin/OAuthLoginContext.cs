using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Models;
using SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Strategies;
using SmartRetail360.Infrastructure.Services.Auth.Login.Shared;
using SmartRetail360.Infrastructure.Services.Auth.Models;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin;

public class OAuthLoginContext : LoginContextBase<OAuthLoginRequest>
{
    public OAuthUserInfo? UserProfile { get; set; }
    public OAuthProviderStrategy OAuthProviderStrategy { get; }

    public OAuthLoginContext(OAuthLoginDependencies dep, OAuthLoginRequest request)
        : base(dep, request)
    {
        OAuthProviderStrategy = dep.OAuthProviderStrategy;
    }
}