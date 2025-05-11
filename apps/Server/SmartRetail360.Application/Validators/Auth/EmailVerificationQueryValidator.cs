using FluentValidation;
using SmartRetail360.Application.DTOs.Auth.Requests;

namespace SmartRetail360.Application.Validators.Auth;

public class EmailVerificationQueryValidator : AbstractValidator<EmailVerificationQuery>
{
    public EmailVerificationQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required")
            .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("Invalid token format");
    }
}