using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.AccountRegistration.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.AccountRegistration;

public class TenantRegisterRequestValidator : AbstractValidator<TenantRegisterRequest>
{
    public TenantRegisterRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.EmailIsRequired))
            .EmailAddress().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidEmailFormat));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordIsRequired))
            .MinimumLength(8).WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustBeAtLeast8Characters))
            .Matches("[A-Z]").WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustContainUppercase))
            .Matches("[a-z]").WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustContainLowercase))
            .Matches("[0-9]").WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustContainDigit))
            .Matches("[^a-zA-Z0-9]").WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustContainSpecialCharacter));
        
        RuleFor(x => x.PasswordConfirmation)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordConfirmationIsRequired))
            .Equal(x => x.Password).WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordsDoNotMatch));
    }
}