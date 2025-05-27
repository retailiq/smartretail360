namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Options;

public class OAuthProviderOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TokenEndpoint { get; set; } = string.Empty;
    public string AuthorizeEndpoint { get; set; } = string.Empty; 
}