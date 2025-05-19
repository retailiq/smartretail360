using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.DTOs.Notification;

public class SendEmailRequest
{
    public string Email { get; set; } = string.Empty;
    public EmailTemplate Template { get; set; }
    public Dictionary<string, string> Variables { get; set; } = new();
}