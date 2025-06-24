namespace SmartRetail360.Contracts.Users.Responses;

public class UpdateUserEmailResponse
{
    public string Email { get; set; }
    public string AccessToken { get; set; } = string.Empty;
}