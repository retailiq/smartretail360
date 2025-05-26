using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Auth;

public class ConfirmTenantLoginRequestValidator : AbstractValidator<ConfirmTenantLoginRequest>
{
    public ConfirmTenantLoginRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.TenantIdIsRequired));

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.UserIdIsRequired));
    }
}