using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Models;
using SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Options;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin.Handlers;

public class FacebookOAuthHandler : IOAuthProviderHandler
{
    private readonly HttpClient _http;
    private readonly OAuthProviderOptions _options;

    private readonly List<string> _uriWhitelist;

    public FacebookOAuthHandler(
        IHttpClientFactory factory,
        IOptions<OAuthOptions> options,
        IOptions<AppOptions> appOptions)
    {
        _http = factory.CreateClient(GeneralConstants.FacebookOAuth);
        _options = options.Value.Providers[OAuthProvider.Facebook];
        _uriWhitelist = appOptions.Value.OAuth.RedirectUriWhitelist;
    }

    public async Task<OAuthUserInfo> GetUserProfileAsync(OAuthLoginRequest request)
    {
        // Step 1: exchange code for access_token
        if (!_uriWhitelist.Contains(request.RedirectUri, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Reason: {LogReasons.OAuthRedirectUriUnauthorized}, Requested URI: {request.RedirectUri}");
        }

        var tokenUrl = _options.TokenEndpoint +
                       $"?client_id={_options.ClientId}" +
                       $"&client_secret={_options.ClientSecret}" +
                       $"&redirect_uri={Uri.EscapeDataString(request.RedirectUri)}" +
                       $"&code={request.Code}";

        var tokenResponse = await _http.GetAsync(tokenUrl);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Reason: {LogReasons.FacebookTokenExchangeFailed}, Status: {tokenResponse.StatusCode}");
        }

        var tokenPayload =
            JsonSerializer.Deserialize<FacebookTokenResponse>(await tokenResponse.Content.ReadAsStringAsync());
        if (tokenPayload == null || string.IsNullOrEmpty(tokenPayload.AccessToken))
        {
            throw new InvalidOperationException($"Reason: {LogReasons.FacebookInvalidTokenResponse}");
        }

        // Step 2: use access_token to get profile info
        var profileResponse = await _http.GetAsync(
            $"{_options.ProfileEndpoint}&access_token={tokenPayload.AccessToken}");

        if (!profileResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Reason: {LogReasons.FacebookProfileFetchFailed}, Status: {profileResponse.StatusCode}");
        }

        var profile =
            JsonSerializer.Deserialize<FacebookProfileResponse>(await profileResponse.Content.ReadAsStringAsync());
        if (profile == null || string.IsNullOrEmpty(profile.Email))
        {
            throw new InvalidOperationException($"Reason: {LogReasons.FacebookInvalidUserProfile}");
        }

        return new OAuthUserInfo
        {
            Email = profile.Email,
            Name = profile.Name,
            AvatarUrl = profile.Picture.Data.IsSilhouette ? string.Empty : profile.Picture.Data.Url,
            ProviderUserId = profile.Id,
            Provider = OAuthProvider.Facebook
        };
    }

    private sealed class FacebookTokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = string.Empty;
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
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