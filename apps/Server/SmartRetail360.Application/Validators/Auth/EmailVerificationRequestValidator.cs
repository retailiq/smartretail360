using FluentValidation;
using SmartRetail360.Contracts.Auth.Requests;

namespace SmartRetail360.Application.Validators.Auth;

public class EmailVerificationRequestValidator : AbstractValidator<EmailVerificationRequest>
{
    public EmailVerificationRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required")
            .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("Invalid token format");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid account type");
    }
}