namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin.Models;

public class OAuthUserProfileResult
{
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }
    public OAuthUserInfo? Profile { get; set; }

    public static OAuthUserProfileResult Success(OAuthUserInfo profile) => new()
    {
        IsSuccess = true,
        Profile = profile
    };

    public static OAuthUserProfileResult Fail(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}