using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Contracts.Notification.Requests;

public class EmailNotificationRequest
{
    public string Email { get; set; } = string.Empty;
    public EmailTemplate Template { get; set; }
}