using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Notifications.Interfaces.Configuration;

public interface IEmailTemplateProvider
{
    string GetSubject(EmailTemplate template);
    string GetBodyHtml(EmailTemplate template, Dictionary<string, string> variables);
}