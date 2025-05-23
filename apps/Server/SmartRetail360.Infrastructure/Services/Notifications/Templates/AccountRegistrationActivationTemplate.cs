using Microsoft.Extensions.Logging;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;

// ReSharper disable All

namespace SmartRetail360.Infrastructure.Services.Notifications.Templates;

public class AccountRegistrationActivationTemplate
{
    private readonly MessageLocalizer _localizer;
    private readonly ILogger<AccountRegistrationActivationTemplate> _logger;

    public AccountRegistrationActivationTemplate(MessageLocalizer localizer,
        ILogger<AccountRegistrationActivationTemplate> logger)
    {
        _localizer = localizer;
        _logger = logger;
    }

    public string GetHtml(Dictionary<string, string> variables)
    {
        var title = string.Format(
            _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationTitle),
            variables.GetValueOrDefault("userName") ?? "User");
        var greeting = _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationGreeting);
        var instruction = _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationInstruction);
        var ctaText = _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationCtaText);
        var footer = _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationFooter);

        return $@"
<!DOCTYPE html>
<html lang=""zh-CN"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            background-color: #f4f6f7;
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 40px auto;
            background-color: #ffffff;
            border-radius: 8px;
            padding: 40px 30px;
            box-shadow: 0 4px 10px rgba(0,0,0,0.05);
        }}
        .logo {{
            text-align: center;
            margin-bottom: 20px;
        }}
        .logo img {{
            width: 120px;
            height: auto;
            display: inline-block;
        }}
        p {{
            color: #4a4a4a;
            font-size: 16px;
            line-height: 1.6;
            margin: 8px 0;
            text-align: left;
        }}
        .button-wrapper {{
            text-align: center;
            margin-top: 30px;
        }}
        a.button {{
            display: inline-block;
            background-color: #2f855a;
            color: white;
            padding: 12px 24px;
            text-decoration: none;
            border-radius: 6px;
            font-weight: bold;
        }}
        .footer {{
            margin-top: 40px;
            font-size: 12px;
            color: #888888;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""logo"">
            <img src=""https://raw.githubusercontent.com/retailiq/smartretail360/6acfa8f10c201c066875f6e22833d4df2dcbcca1/apps/client/public/logo_no_bg.png"" alt=""Company Logo"" />
        </div>

        <p>{title}</p>
        <p>{greeting}</p>
        <p>{instruction}</p>

        <div class=""button-wrapper"">
            <a class=""button"" href=""{variables["activation_link"]}"">{ctaText}</a>
        </div>

        <div class=""footer"">
            {footer}
        </div>
    </div>
</body>
</html>
";
    }
}