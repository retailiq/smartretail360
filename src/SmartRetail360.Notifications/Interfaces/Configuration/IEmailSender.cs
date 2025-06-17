using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Notifications.Interfaces.Configuration;

public interface IEmailSender
{
    Task SendAsync(string to, EmailTemplate template, Dictionary<string, string> variables);
}