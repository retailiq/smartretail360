using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Contracts.Users.Requests;

public class UpdateUserBasicProfileRequest
{
    public string? Name { get; set; }
    public string? CountryCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public LocaleType? Locale { get; set; }
}