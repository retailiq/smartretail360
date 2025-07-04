namespace SmartRetail360.Contracts.Users.Responses;

public class UpdateUserEmailResponse
{
    public string NewEmail { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}