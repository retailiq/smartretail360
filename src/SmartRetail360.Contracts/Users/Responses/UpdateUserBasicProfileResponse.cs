namespace SmartRetail360.Contracts.Users.Responses;

public class UpdateUserBasicProfileResponse
{
    public string? Name { get; set; }
    public string? CountryCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Locale { get; set; }
    public string AccessToken { get; set; } = string.Empty;
}