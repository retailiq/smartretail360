using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Users.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Users;

public class UpdateUserEmailRequestValidator : AbstractValidator<UpdateUserEmailRequest>
{
    public UpdateUserEmailRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.NewEmailIsRequired))
            .EmailAddress().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidEmailFormat))
            .WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.NewEmailMustBeDifferentFromCurrent));
    }
}