using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Notifications;

public interface IEmailSender
{
    Task SendAsync(string to, EmailTemplate template, Dictionary<string, string> variables);
}