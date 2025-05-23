using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Auth;

public class EmailVerificationRequestValidator : AbstractValidator<EmailVerificationRequest>
{
    public EmailVerificationRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.TokenIsRequired))
            .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidTokenFormat));
    }
}