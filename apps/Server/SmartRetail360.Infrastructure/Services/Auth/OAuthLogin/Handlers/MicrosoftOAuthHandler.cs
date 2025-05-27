using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Models;
using SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Options;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Handlers;

public class MicrosoftOAuthHandler : IOAuthProviderHandler
{
    private readonly HttpClient _http;
    private readonly OAuthProviderOptions _options;
    private readonly ILogger<MicrosoftOAuthHandler> _logger;

    public MicrosoftOAuthHandler(
        IHttpClientFactory factory, 
        IOptions<OAuthOptions> options, 
        ILogger<MicrosoftOAuthHandler> logger,
        IOptions<AppOptions> appOptions)
    {
        _logger = logger;
        var tenant = appOptions.Value.OAuth.Tenant;
        var rawOptions = options.Value.Providers[OAuthProvider.Microsoft];
        _options = new OAuthProviderOptions
        {
            ClientId = rawOptions.ClientId,
            ClientSecret = rawOptions.ClientSecret,
            TokenEndpoint = rawOptions.TokenEndpoint.Replace("{tenant}", tenant),
            AuthorizeEndpoint = rawOptions.AuthorizeEndpoint.Replace("{tenant}", tenant)
        };
        
        _http = factory.CreateClient(GeneralConstants.MicrosoftOAuth);
      
    }

    public async Task<OAuthUserProfileResult> GetUserProfileAsync(OAuthLoginRequest request)
    {
        // Step 1: code → access token
        var tokenRequest = new Dictionary<string, string>
        {
            { "client_id", _options.ClientId },
            { "scope", "User.Read" },
            { "code", request.Code },
            { "redirect_uri", request.RedirectUri },
            { "grant_type", "authorization_code" },
            { "client_secret", _options.ClientSecret }
        };
        
        var tokenHttpRequest = new HttpRequestMessage(HttpMethod.Post, _options.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(tokenRequest)
        };
        
        var tokenResponse = await _http.SendAsync(tokenHttpRequest);
        if (!tokenResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("[OAuth:Microsoft] Token exchange failed. Status: {Status}", tokenResponse.StatusCode);
            return OAuthUserProfileResult.Fail("Microsoft token exchange failed");
        }

        var tokenPayload = JsonSerializer.Deserialize<MicrosoftTokenResponse>(await tokenResponse.Content.ReadAsStringAsync());
        if (tokenPayload == null || string.IsNullOrEmpty(tokenPayload.AccessToken))
            return OAuthUserProfileResult.Fail("Invalid Microsoft token response");

        // Step 2: access_token → profile
        var profileRequest = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
        profileRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenPayload.AccessToken);
        var profileResponse = await _http.SendAsync(profileRequest);
        if (!profileResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("[OAuth:Microsoft] Profile fetch failed. Status: {Status}", profileResponse.StatusCode);
            return OAuthUserProfileResult.Fail("Microsoft profile fetch failed");
        }

        var profile = JsonSerializer.Deserialize<MicrosoftProfileResponse>(await profileResponse.Content.ReadAsStringAsync());
        if (profile == null || string.IsNullOrEmpty(profile.Mail ?? profile.UserPrincipalName))
            return OAuthUserProfileResult.Fail("Invalid Microsoft user profile");

        return OAuthUserProfileResult.Success(new OAuthUserInfo
        {
            Email = profile.Mail ?? profile.UserPrincipalName ?? string.Empty,
            Name = profile.DisplayName,
            AvatarUrl = string.Empty,
            ProviderUserId = profile.Id,      
            Provider = OAuthProvider.Microsoft   
        });
    }

    private sealed class MicrosoftTokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = string.Empty;
    }

    private sealed class MicrosoftProfileResponse
    {
        [JsonPropertyName("displayName")] public string DisplayName { get; set; } = string.Empty;
        [JsonPropertyName("mail")] public string? Mail { get; set; }
        [JsonPropertyName("userPrincipalName")] public string? UserPrincipalName { get; set; }
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    }
}
