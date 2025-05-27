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

namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Handlers;

public class GoogleOAuthHandler : IOAuthProviderHandler
{
    private readonly HttpClient _http;
    private readonly OAuthProviderOptions _options;
    private readonly ILogger<GoogleOAuthHandler> _logger;

    public GoogleOAuthHandler(
        IHttpClientFactory factory, 
        IOptions<OAuthOptions> options, 
        ILogger<GoogleOAuthHandler> logger)
    {
        _http = factory.CreateClient(GeneralConstants.GoogleOAuth);
        _options = options.Value.Providers[OAuthProvider.Google];
        _logger = logger;
    }

    public async Task<OAuthUserProfileResult> GetUserProfileAsync(OAuthLoginRequest request)
    {
        // Step 1: exchange code -> token
        var tokenRequest = new Dictionary<string, string>
        {
            { "code", request.Code },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret },
            { "redirect_uri", request.RedirectUri },
            { "grant_type", "authorization_code" }
        };

        var tokenResponse = await _http.PostAsync("/token", new FormUrlEncodedContent(tokenRequest));
        if (!tokenResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("[OAuth:Google] Token exchange failed. Status: {Status}", tokenResponse.StatusCode);
            return OAuthUserProfileResult.Fail("OAuth token exchange failed");
        }

        var tokenPayload = JsonSerializer.Deserialize<GoogleTokenResponse>(await tokenResponse.Content.ReadAsStringAsync());
        if (tokenPayload == null || string.IsNullOrEmpty(tokenPayload.AccessToken))
            return OAuthUserProfileResult.Fail("Invalid token response");

        // Step 2: access token -> user profile
        var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
        userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenPayload.AccessToken);
        var userResponse = await _http.SendAsync(userRequest);
        if (!userResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("[OAuth:Google] Failed to get user profile. Status: {Status}", userResponse.StatusCode);
            return OAuthUserProfileResult.Fail("OAuth profile retrieval failed");
        }

        var profile = JsonSerializer.Deserialize<GoogleProfileResponse>(await userResponse.Content.ReadAsStringAsync());
        if (profile == null || string.IsNullOrEmpty(profile.Email))
            return OAuthUserProfileResult.Fail("Invalid user profile");

        return OAuthUserProfileResult.Success(new OAuthUserInfo
        {
            Email = profile.Email,
            Name = profile.Name,
            AvatarUrl = profile.Picture,
            ProviderUserId = profile.Id,
            Provider = OAuthProvider.Google
        });
    }

    private sealed class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = string.Empty;
    }

    private sealed class GoogleProfileResponse
    {
        [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("picture")] public string Picture { get; set; } = string.Empty;
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    }
}
