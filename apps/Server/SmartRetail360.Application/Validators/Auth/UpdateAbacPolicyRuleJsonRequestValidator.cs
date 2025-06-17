using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Auth;

public class UpdateAbacPolicyRuleJsonRequestValidator : AbstractValidator<UpdateAbacPolicyRuleJsonRequest>
{
    public UpdateAbacPolicyRuleJsonRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.RuleJson)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.AbacPolicyRuleJsonRequired));
    }
}