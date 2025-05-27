using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Options;

public class OAuthOptions
{
    public Dictionary<OAuthProvider, OAuthProviderOptions> Providers { get; set; } = new();
}