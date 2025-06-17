using SmartRetail360.Notifications.Interfaces.Configuration;
using SmartRetail360.Notifications.Services.Templates;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;

namespace SmartRetail360.Notifications.Services.Configuration;

public class DefaultEmailTemplateProvider : IEmailTemplateProvider
{
    private readonly AccountRegistrationActivationTemplate _accountRegistrationActivationTemplate;
    private readonly MessageLocalizer _localizer;

    public DefaultEmailTemplateProvider(
        AccountRegistrationActivationTemplate accountRegistrationActivationTemplate,
        MessageLocalizer localizer)
    {
        _accountRegistrationActivationTemplate = accountRegistrationActivationTemplate;
        _localizer = localizer;
    }

    public string GetSubject(EmailTemplate template) => template switch
    {
        EmailTemplate.UserRegistrationActivation => _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationSubject),
        EmailTemplate.PasswordReset => "Reset your password",
        EmailTemplate.VerificationCode => "Your verification code",
        EmailTemplate.Marketing => "Exclusive offer just for you!",
        _ => "Notification"
    };

    public string GetBodyHtml(EmailTemplate template, Dictionary<string, string> variables)
    {
        return template switch
        {
            EmailTemplate.UserRegistrationActivation => _accountRegistrationActivationTemplate.GetHtml(variables),

            EmailTemplate.VerificationCode =>
                $"<p>您的验证码是：<strong>{variables["code"]}</strong></p>",

            _ => "<p>默认邮件内容</p>"
        };
    }
}