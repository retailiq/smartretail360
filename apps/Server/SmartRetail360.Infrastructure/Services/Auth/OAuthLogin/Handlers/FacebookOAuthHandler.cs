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

public class FacebookOAuthHandler : IOAuthProviderHandler
{
    private readonly HttpClient _http;
    private readonly OAuthProviderOptions _options;
    private readonly ILogger<FacebookOAuthHandler> _logger;

    public FacebookOAuthHandler(IHttpClientFactory factory, IOptions<OAuthOptions> options,
        ILogger<FacebookOAuthHandler> logger)
    {
        _http = factory.CreateClient(GeneralConstants.FacebookOAuth);
        _options = options.Value.Providers[OAuthProvider.Facebook];
        _logger = logger;
    }

    public async Task<OAuthUserProfileResult> GetUserProfileAsync(OAuthLoginRequest request)
    {
        // Step 1: exchange code for access_token
        var tokenResponse = await _http.GetAsync($"/v18.0/oauth/access_token" +
                                                 $"?client_id={_options.ClientId}" +
                                                 $"&client_secret={_options.ClientSecret}" +
                                                 $"&redirect_uri={Uri.EscapeDataString(request.RedirectUri)}" +
                                                 $"&code={request.Code}");

        if (!tokenResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("[OAuth:Facebook] Token exchange failed. Status: {Status}", tokenResponse.StatusCode);
            return OAuthUserProfileResult.Fail("Facebook token exchange failed");
        }

        var tokenPayload =
            JsonSerializer.Deserialize<FacebookTokenResponse>(await tokenResponse.Content.ReadAsStringAsync());
        if (tokenPayload == null || string.IsNullOrEmpty(tokenPayload.AccessToken))
            return OAuthUserProfileResult.Fail("Invalid Facebook token response");

        // Step 2: use access_token to get profile info
        var profileResponse =
            await _http.GetAsync(
                $"/me?fields=id,name,email,picture.width(256)&access_token={tokenPayload.AccessToken}");
        if (!profileResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("[OAuth:Facebook] Failed to get user profile. Status: {Status}",
                profileResponse.StatusCode);
            return OAuthUserProfileResult.Fail("Facebook profile fetch failed");
        }
        var profile =
            JsonSerializer.Deserialize<FacebookProfileResponse>(await profileResponse.Content.ReadAsStringAsync());
        if (profile == null || string.IsNullOrEmpty(profile.Email))
            return OAuthUserProfileResult.Fail("Invalid Facebook user profile");

        return OAuthUserProfileResult.Success(new OAuthUserInfo
        {
            Email = profile.Email,
            Name = profile.Name,
            AvatarUrl = profile.Picture.Data.IsSilhouette ? string.Empty : profile.Picture.Data.Url,
            ProviderUserId = profile.Id,
            Provider = OAuthProvider.Facebook
        });
    }

    private sealed class FacebookTokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = string.Empty;
    }

    private sealed class FacebookProfileResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
        [JsonPropertyName("picture")] public FacebookPicture Picture { get; set; } = new();

        public class FacebookPicture
        {
            [JsonPropertyName("data")] public PictureData Data { get; set; } = new();
        }

        public class PictureData
        {
            [JsonPropertyName("url")] public string Url { get; set; } = string.Empty;
            [JsonPropertyName("is_silhouette")] public bool IsSilhouette { get; set; }
        }
    }
}