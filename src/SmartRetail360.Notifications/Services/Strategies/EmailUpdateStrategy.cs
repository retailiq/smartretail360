using Microsoft.Extensions.Options;
using SmartRetail360.Notifications.Interfaces.Configuration;
using SmartRetail360.Notifications.Interfaces.Strategies;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Notifications.Services.Strategies;

public class EmailUpdateStrategy : IEmailStrategy
{
    private readonly IEmailSender _emailSender;
    private readonly AppOptions _appOptions;

    public EmailTemplate StrategyKey => EmailTemplate.EmailUpdate;

    public EmailUpdateStrategy(
        IEmailSender emailSender,
        IOptions<AppOptions> options)
    {
        _emailSender = emailSender;
        _appOptions = options.Value;
    }

    public async Task ExecuteAsync(string toEmail, IDictionary<string, string> data)
    {
        var link = UrlBuilder.BuildApiUrl(
            _appOptions.FrontendUrl,
            path: _appOptions.EmailVerificationUrl,
            queryParams: new()
            {
                ["token"] = data["token"],
                ["locale"] = data.GetOrDefault("locale", "en"),
                ["traceId"] = data["traceId"],
                ["timestamp"] = data["timestamp"]
            });
        var variables = new Dictionary<string, string>(data) { ["activation_link"] = link };
        await _emailSender.SendAsync(toEmail, EmailTemplate.EmailUpdate, variables);
    }
}