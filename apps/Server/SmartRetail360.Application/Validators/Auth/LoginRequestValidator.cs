using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.EmailIsRequired))
            .EmailAddress().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidEmailFormat));
    
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordIsRequired));
    }
}