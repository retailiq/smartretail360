using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Auth;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.IsStaySignedIn)
            .NotNull().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.IsStaySignedInIsRequired));
    }
}