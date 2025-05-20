using FluentValidation;
using SmartRetail360.Contracts.AccountRegistration.Requests;

namespace SmartRetail360.Application.Validators;

public class TenantRegisterRequestValidator : AbstractValidator<TenantRegisterRequest>
{
    public TenantRegisterRequestValidator()
    {
        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Must contain uppercase letter")
            .Matches("[a-z]").WithMessage("Must contain lowercase letter")
            .Matches("[0-9]").WithMessage("Must contain digit");

        RuleFor(x => x.PasswordConfirmation)
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}