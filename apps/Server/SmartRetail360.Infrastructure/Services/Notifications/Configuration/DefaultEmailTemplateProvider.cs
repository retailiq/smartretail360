using SmartRetail360.Application.Interfaces.Notifications.Configuration;
using SmartRetail360.Infrastructure.Services.Notifications.Templates;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;

namespace SmartRetail360.Infrastructure.Services.Notifications.Configuration;

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
        EmailTemplate.AccountRegistrationActivation => _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationSubject),
        EmailTemplate.PasswordReset => "重置您的密码",
        EmailTemplate.VerificationCode => "您的验证码",
        EmailTemplate.Marketing => "为您推荐好物",
        _ => "通知"
    };

    public string GetBodyHtml(EmailTemplate template, Dictionary<string, string> variables)
    {
        return template switch
        {
            EmailTemplate.AccountRegistrationActivation => _accountRegistrationActivationTemplate.GetHtml(variables),

            EmailTemplate.VerificationCode =>
                $"<p>您的验证码是：<strong>{variables["code"]}</strong></p>",

            _ => "<p>默认邮件内容</p>"
        };
    }
}