using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.DTOs.Notification;

public class EmailNotificationRequest
{
    public string Email { get; set; } = string.Empty;
    public EmailTemplate Template { get; set; }
}