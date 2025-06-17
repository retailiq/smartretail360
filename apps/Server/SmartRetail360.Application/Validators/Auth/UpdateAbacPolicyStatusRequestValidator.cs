using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Auth;

public class UpdateAbacPolicyStatusRequestValidator : AbstractValidator<UpdateAbacPolicyStatusRequest>
{
    public UpdateAbacPolicyStatusRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.IsEnabled)
            .NotNull().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.AbacPolicyStatusRequired));
    }
}