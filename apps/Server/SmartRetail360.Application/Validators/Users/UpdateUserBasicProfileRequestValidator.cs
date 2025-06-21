using FluentValidation;
using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Users.Requests;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Validators.Users;

public class UpdateUserBasicProfileRequestValidator : AbstractValidator<UpdateUserBasicProfileRequest>
{
    public UpdateUserBasicProfileRequestValidator(ApplicationDependencies dep)
    {
        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .MinimumLength(1)
                .WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.NameMustBeAtLeast1Characters))
                .MaximumLength(50)
                .WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.NameMustNotExceed50Characters));
        });

        RuleFor(x => x)
            .Custom((x, context) =>
            {
                var phone = x.PhoneNumber;
                var code = x.CountryCode;

                var hasPhone = !string.IsNullOrWhiteSpace(phone);
                var hasCode = !string.IsNullOrWhiteSpace(code);

                if (hasPhone ^ hasCode)
                {
                    context.AddFailure(
                        dep.Localizer.GetLocalizedText(LocalizedTextKey.PhoneAndCountryCodeMustBeTogether));
                }
            });
        
        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber!)
                .Matches(@"^\d{6,15}$")
                .WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidPhoneNumberFormat));
        });

        When(x => !string.IsNullOrWhiteSpace(x.CountryCode), () =>
        {
            RuleFor(x => x.CountryCode!)
                .Matches(@"^\+\d{1,4}$")
                .WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidCountryCodeFormat));
        });

        When(x => x.AvatarUrl != null, () =>
        {
            RuleFor(x => x.AvatarUrl)
                .Must(IsValidUrl).WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidUrlFormat));
        });

        When(x => x.Locale != null, () =>
        {
            RuleFor(x => x.Locale)
                .IsInEnum().WithMessage(dep.Localizer.GetLocalizedText(LocalizedTextKey.InvalidLocale));
        });
    }

    private static bool IsValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var _) &&
               (url.StartsWith("http://") || url.StartsWith("https://"));
    }
}