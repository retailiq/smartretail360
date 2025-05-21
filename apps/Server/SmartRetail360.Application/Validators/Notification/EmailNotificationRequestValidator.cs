using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Notification.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Notification;

public class EmailNotificationRequestValidator : AbstractValidator<EmailNotificationRequest>
{
    public EmailNotificationRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.EmailIsRequired))
            .EmailAddress().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidEmailFormat));

        RuleFor(x => x.Template)
            .IsInEnum().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidEmailTemplate));
    }
}