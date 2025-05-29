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

public class MicrosoftOAuthHandler : IOAuthProviderHandler
{
    private readonly HttpClient _http;
    private readonly OAuthProviderOptions _options;

    private readonly List<string> _uriWhitelist;
    
    public MicrosoftOAuthHandler(
        IHttpClientFactory factory,
        IOptions<OAuthOptions> options,
        IOptions<AppOptions> appOptions)
    {
        var tenant = appOptions.Value.OAuth.Tenant;
        var rawOptions = options.Value.Providers[OAuthProvider.Microsoft];
        _options = new OAuthProviderOptions
        {
            ClientId = rawOptions.ClientId,
            ClientSecret = rawOptions.ClientSecret,
            TokenEndpoint = rawOptions.TokenEndpoint.Replace("{tenant}", tenant),
            ProfileEndpoint = rawOptions.ProfileEndpoint
        };
        _uriWhitelist = appOptions.Value.OAuth.RedirectUriWhitelist;
        _http = factory.CreateClient(GeneralConstants.MicrosoftOAuth);
    }

    public async Task<OAuthUserInfo> GetUserProfileAsync(OAuthLoginRequest request)
    {
        // Step 1: exchange code for access_token
        if (!_uriWhitelist.Contains(request.RedirectUri, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Reason: {LogReasons.OAuthRedirectUriUnauthorized}, Requested URI: {request.RedirectUri}");
        }
        
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
            throw new InvalidOperationException(
                $"Reason: {LogReasons.MicrosoftTokenExchangeFailed}, Status: {tokenResponse.StatusCode}");
        }

        var tokenPayload = JsonSerializer.Deserialize<MicrosoftTokenResponse>(
            await tokenResponse.Content.ReadAsStringAsync());
        if (tokenPayload == null || string.IsNullOrEmpty(tokenPayload.AccessToken))
        {
            throw new InvalidOperationException($"Reason: {LogReasons.MicrosoftInvalidTokenResponse}");
        }

        // Step 2: access_token → profile
        var profileRequest = new HttpRequestMessage(HttpMethod.Get, _options.ProfileEndpoint);
        profileRequest.Headers.Authorization = new AuthenticationHeaderValue(GeneralConstants.Bearer, tokenPayload.AccessToken);
        var profileResponse = await _http.SendAsync(profileRequest);

        if (!profileResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Reason: {LogReasons.MicrosoftProfileFetchFailed}, Status: {profileResponse.StatusCode}");
        }

        var profile = JsonSerializer.Deserialize<MicrosoftProfileResponse>(
            await profileResponse.Content.ReadAsStringAsync());
        if (profile == null || string.IsNullOrEmpty(profile.Mail ?? profile.UserPrincipalName))
        {
            throw new InvalidOperationException($"Reason: {LogReasons.MicrosoftInvalidUserProfile}");
        }

        return new OAuthUserInfo
        {
            Email = profile.Mail ?? profile.UserPrincipalName ?? string.Empty,
            Name = profile.DisplayName,
            AvatarUrl = string.Empty,
            ProviderUserId = profile.Id,
            Provider = OAuthProvider.Microsoft
        };
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