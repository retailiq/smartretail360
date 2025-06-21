namespace SmartRetail360.Contracts.Users.Requests;

public class UpdateUserEmailRequest
{
    public string CurrentEmail { get; set; }
    public string NewEmail { get; set; }
}