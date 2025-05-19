using FluentValidation;
using SmartRetail360.Application.DTOs.Auth.Requests;
using SmartRetail360.Application.DTOs.Notification;

namespace SmartRetail360.Application.Validators.Notification;

public class EmailNotificationRequestValidator : AbstractValidator<EmailNotificationRequest>
{
    public EmailNotificationRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Template)
            .IsInEnum().WithMessage("Invalid email template.");
    }
}