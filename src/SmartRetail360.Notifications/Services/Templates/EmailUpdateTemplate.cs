using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using Microsoft.Extensions.Options;
using SmartRetail360.Shared.Options;
using System.Net;
using SmartRetail360.Shared.Constants;

namespace SmartRetail360.Notifications.Services.Templates
{
    public class EmailUpdateTemplate
    {
        private readonly MessageLocalizer _localizer;
        private readonly AppOptions _appOptions;

        public EmailUpdateTemplate(
            MessageLocalizer localizer,
            IOptions<AppOptions> appOptions)
        {
            _localizer = localizer;
            _appOptions = appOptions.Value;
        }

        public string GetHtml(Dictionary<string, string> variables)
        {
            var userName = variables.GetValueOrDefault("userName") ?? GeneralConstants.User;
            var oldEmail = variables.GetValueOrDefault("oldEmail") ?? GeneralConstants.Unknown;
            var newEmail = variables.GetValueOrDefault("newEmail") ?? GeneralConstants.Unknown;
            var minutes = variables.GetValueOrDefault("emailValidationMinutes") ?? "15";

            var activationLinkRaw = variables.GetValueOrDefault("activation_link") ?? "#";
            var activationLinkEncoded = WebUtility.HtmlEncode(activationLinkRaw);
            
            var greeting = string.Format(_localizer.GetLocalizedText(LocalizedTextKey.EmailUpdateEmailSendingGreeting), userName);
            var instruction = string.Format(_localizer.GetLocalizedText(LocalizedTextKey.EmailUpdateEmailSendingInstruction), oldEmail, newEmail);
            var ctaText = _localizer.GetLocalizedText(LocalizedTextKey.EmailUpdateEmailSendingCtaText);
            var validityNotice = string.Format(_localizer.GetLocalizedText(LocalizedTextKey.EmailUpdateEmailSendingValidityNotice), minutes);
            var manualInstruction = _localizer.GetLocalizedText(LocalizedTextKey.EmailUpdateEmailSendingManualLinkInstruction);
            var footer = _localizer.GetLocalizedText(LocalizedTextKey.EmailUpdateEmailSendingFooter);

            return $@"
            <!DOCTYPE html>
            <html xmlns=""http://www.w3.org/1999/xhtml"" lang=""zh-CN"">
            <head>
                <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
                <title>Email Update Confirmation</title>
                <style type=""text/css"">
                    body, html {{
                        margin: 0 !important;
                        padding: 0 !important;
                        height: 100% !important;
                        width: 100% !important;
                        background-color: #f4f6f7 !important;
                        font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Helvetica, Arial, sans-serif !important;
                    }}
                    .email-container {{
                        max-width: 600px !important;
                        margin: 0 auto !important;
                        background-color: #ffffff !important;
                        box-shadow: 0 4px 10px rgba(0,0,0,0.05) !important;
                        padding: 40px 30px !important;
                    }}
                    .logo {{
                        text-align: center !important;
                        padding-bottom: 20px !important;
                    }}
                    .logo img {{
                        width: 120px !important;
                        display: block !important;
                        margin: 0 auto !important;
                    }}
                    p {{
                        font-size: 16px !important;
                        line-height: 1.6 !important;
                        color: #4a4a4a !important;
                        margin: 8px 0 !important;
                    }}
                    .button-wrapper {{
                        text-align: center !important;
                        padding-top: 30px !important;
                    }}
                    a.button {{
                        display: inline-block !important;
                        background-color: #2f855a !important;
                        color: white !important;
                        padding: 12px 24px !important;
                        text-decoration: none !important;
                        border-radius: 6px !important;
                        font-weight: bold !important;
                    }}
                    .footer {{
                        padding-top: 40px !important;
                        font-size: 12px !important;
                        color: #888888 !important;
                        text-align: center !important;
                    }}
                    .notice {{
                        color: #888888 !important;
                        font-size: 14px !important;
                        font-style: italic !important;
                        margin: 12px 0 !important;
                    }}
                </style>
            </head>
            <body>
                <div class=""email-container"">
                    <div class=""logo"">
                        <img src=""{_appOptions.LogoUrl}"" alt=""Company Logo"" />
                    </div>

                    <p>{greeting}</p>
                    <p>{instruction}</p>
                    <p class=""notice"">{validityNotice}</p>

                    <div class=""button-wrapper"">
                        <a class=""button"" href=""{activationLinkEncoded}"" rel=""noopener noreferrer"">{ctaText}</a>
                    </div>

                    <div style=""font-size: 14px; color: #888888; font-style: italic; margin: 12px 0;"">
                        <p>{manualInstruction}</p>
                        <div style=""word-break: break-all; overflow-wrap: break-word;"">
                            <a href=""{activationLinkEncoded}"" style=""color:#2f855a;"">{activationLinkEncoded}</a>
                        </div>
                    </div>

                    <div class=""footer"">
                        {footer}
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}