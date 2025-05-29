using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Auth;

public class OAuthLoginRequestValidator : AbstractValidator<OAuthLoginRequest>
{
    public OAuthLoginRequestValidator(ApplicationDependencies dep)
    {
        RuleFor(x => x.Provider)
            .IsInEnum().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidOAuthProvider))
            .NotEqual(OAuthProvider.None).WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidOAuthProvider));
        
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.OAuthCodeIsRequired));

        RuleFor(x => x.OAuthState)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.OAuthStateIsRequired));

        RuleFor(x => x.RedirectUri)
            .NotEmpty().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.OAuthRedirectUriIsRequired));
    }
}