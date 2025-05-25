using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.AccountRegistration.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.AccountRegistration;

public class AccountRegisterRequestValidator : AbstractValidator<AccountRegisterRequest>
{
    public AccountRegisterRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.EmailIsRequired))
            .EmailAddress().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidEmailFormat));

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.NameIsRequired))
            .MinimumLength(1).WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.NameMustBeAtLeast1Characters))
            .MaximumLength(50).WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.NameMustNotExceed50Characters));
        
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