using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using Microsoft.Extensions.Options;
using SmartRetail360.Shared.Options;

namespace SmartRetail360.Notifications.Services.Templates
{
    public class AccountRegistrationActivationTemplate
    {
        private readonly MessageLocalizer _localizer;
        private readonly AppOptions _appOptions;

        public AccountRegistrationActivationTemplate(
            MessageLocalizer localizer,
            IOptions<AppOptions> appOptions)
        {
            _localizer = localizer;
            _appOptions = appOptions.Value;
        }

        public string GetHtml(Dictionary<string, string> variables)
        {
            var userName = variables.GetValueOrDefault("userName") ?? "User";
            var minutes = variables.GetValueOrDefault("emailValidationMinutes") ?? "15";

            var title = string.Format(
                _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationTitle),
                userName);
            var greeting = _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationGreeting);
            var instruction = _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationInstruction);
            var ctaText = _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationCtaText);
            var validityNotice =
                string.Format(_localizer.GetLocalizedText(LocalizedTextKey.AccountActivationValidityNotice), minutes);
            var footer = _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationFooter);
            
            var manualInstruction = _localizer.GetLocalizedText(LocalizedTextKey.AccountActivationManualLinkInstruction);
            var activationLinkRaw = variables.GetValueOrDefault("activation_link") ?? "#";
            var activationLinkEncoded = System.Net.WebUtility.HtmlEncode(activationLinkRaw);

            return $@"
            <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
            <html xmlns=""http://www.w3.org/1999/xhtml"" lang=""zh-CN"">
            <head>
                <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
                <title>Account Activation</title>
                <style type=""text/css"">
                    body, html {{
                        margin: 0 !important;
                        padding: 0 !important;
                        height: 100% !important;
                        width: 100% !important;
                        -webkit-text-size-adjust: 100% !important;
                        -ms-text-size-adjust: 100% !important;
                        background-color: #f4f6f7 !important;
                        font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Helvetica, Arial, sans-serif !important;
                    }}

                    table, table td, table tr {{
                        border-collapse: collapse !important;
                        border-spacing: 0 !important;
                        mso-table-lspace: 0pt !important;
                        mso-table-rspace: 0pt !important;
                        margin: 0 !important;
                        padding: 0 !important;
                    }}

                    .email-root {{
                        width: 100% !important;
                        height: 100% !important;
                        margin: 0 !important;
                        padding: 0 !important;
                        background-color: #f4f6f7 !important;
                    }}

                    .email-container {{
                        width: 100% !important;
                        max-width: 600px !important;
                        margin: 0 auto !important;
                        background-color: #ffffff !important;
                        box-shadow: 0 4px 10px rgba(0,0,0,0.05) !important;
                    }}

                    .email-content {{
                        padding: 40px 30px !important;
                    }}

                    .logo {{
                        text-align: center !important;
                        padding-bottom: 20px !important;
                    }}

                    .logo img {{
                        width: 120px !important;
                        height: auto !important;
                        display: block !important;
                        margin: 0 auto !important;
                    }}

                    p {{
                        font-size: 16px !important;
                        line-height: 1.6 !important;
                        margin: 8px 0 !important;
                        text-align: left !important;
                        color: #4a4a4a !important;
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
            <body style=""margin:0 !important; padding:0 !important; width:100% !important;"">
                <table class=""email-root"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                    <tr>
                        <td align=""center"" valign=""top"">
                            <!--[if (gte mso 9)|(IE)]>
                            <table width=""600"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                            <tr>
                            <td>
                            <![endif]-->

                            <table class=""email-container"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                    <td class=""email-content"">
                                        <div class=""logo"">
                                            <img src=""{_appOptions.LogoUrl}"" alt=""Company Logo"" />
                                        </div>

                                        <p>{title}</p>
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
                                    </td>
                                </tr>
                            </table>

                            <!--[if (gte mso 9)|(IE)]>
                            </td>
                            </tr>
                            </table>
                            <![endif]-->
                        </td>
                    </tr>
                </table>
            </body>
            </html>";
        }
    }
}