using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Users.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Users;

public class UpdateUserPasswordRequestValidator: AbstractValidator<UpdateUserPasswordRequest>
{
    public UpdateUserPasswordRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.CurrentPasswordIsRequired));

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.NewPasswordIsRequired))
            .MinimumLength(8)
            .WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustBeAtLeast8Characters))
            .Matches("[A-Z]").WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustContainUppercase))
            .Matches("[a-z]").WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustContainLowercase))
            .Matches("[0-9]").WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustContainDigit))
            .Matches("[^a-zA-Z0-9]")
            .WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustContainSpecialCharacter))
            .NotEqual(x => x.CurrentPassword)
            .WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordMustBeDifferentFromCurrent));
        
        RuleFor(x => x.NewPasswordConfirmation)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.NewPasswordConfirmationIsRequired))
            .Equal(x => x.NewPassword).WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.PasswordsDoNotMatch));
    }
}