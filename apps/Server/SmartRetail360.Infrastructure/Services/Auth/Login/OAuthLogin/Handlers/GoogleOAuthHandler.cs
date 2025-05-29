using System.Net.Http.Headers;
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

public class GoogleOAuthHandler : IOAuthProviderHandler
{
    private readonly HttpClient _http;
    private readonly OAuthProviderOptions _options;

    private readonly List<string> _uriWhitelist;

    public GoogleOAuthHandler(
        IHttpClientFactory factory,
        IOptions<OAuthOptions> options,
        IOptions<AppOptions> appOptions)
    {
        _http = factory.CreateClient(GeneralConstants.GoogleOAuth);
        _options = options.Value.Providers[OAuthProvider.Google];
        _uriWhitelist = appOptions.Value.OAuth.RedirectUriWhitelist;
    }

    public async Task<OAuthUserInfo> GetUserProfileAsync(OAuthLoginRequest request)
    {
        // Step 1: exchange code -> token
        // var redirectUri = $"{_appOptions.BaseUrl}/auth/callback/{request.Provider.GetEnumMemberValue()}";
        if (!_uriWhitelist.Contains(request.RedirectUri, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Reason: {LogReasons.OAuthRedirectUriUnauthorized}, Requested URI: {request.RedirectUri}");
        }

        var tokenRequest = new Dictionary<string, string>
        {
            { "code", request.Code },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret },
            { "redirect_uri", request.RedirectUri },
            { "grant_type", "authorization_code" }
        };

        var tokenResponse = await _http.PostAsync(
            _options.TokenEndpoint,
            new FormUrlEncodedContent(tokenRequest));
        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Reason: {LogReasons.GoogleTokenExchangeFailed}, Status: {tokenResponse.StatusCode}");
        }

        var tokenPayload = JsonSerializer.Deserialize<GoogleTokenResponse>(
            await tokenResponse.Content.ReadAsStringAsync());
        if (tokenPayload == null || string.IsNullOrEmpty(tokenPayload.AccessToken))
        {
            throw new InvalidOperationException($"Reason: {LogReasons.GoogleInvalidTokenResponse}");
        }

        // Step 2: access token -> user profile
        var userRequest = new HttpRequestMessage(HttpMethod.Get, _options.ProfileEndpoint);
        userRequest.Headers.Authorization =
            new AuthenticationHeaderValue(GeneralConstants.Bearer, tokenPayload.AccessToken);
        var userResponse = await _http.SendAsync(userRequest);
        if (!userResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Reason: {LogReasons.GoogleProfileFetchFailed}, Status: {userResponse.StatusCode}");
        }

        var profile = JsonSerializer.Deserialize<GoogleProfileResponse>(
            await userResponse.Content.ReadAsStringAsync());
        if (profile == null || string.IsNullOrEmpty(profile.Email))
        {
            throw new InvalidOperationException($"Reason: {LogReasons.GoogleInvalidUserProfile}");
        }

        return new OAuthUserInfo
        {
            Email = profile.Email,
            Name = profile.Name,
            AvatarUrl = profile.Picture,
            ProviderUserId = profile.Id,
            Provider = OAuthProvider.Google
        };
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