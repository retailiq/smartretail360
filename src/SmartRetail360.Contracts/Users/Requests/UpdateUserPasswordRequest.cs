namespace SmartRetail360.Contracts.Users.Requests;

public class UpdateUserPasswordRequest
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string NewPasswordConfirmation { get; set; }
}