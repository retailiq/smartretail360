using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Notifications.Strategies;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Utils;
using Microsoft.Extensions.Options;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Infrastructure.Services.Notifications.Strategies;

public class UserAccountActivationEmailStrategy : IEmailStrategy
{
    private readonly IEmailSender _emailSender;
    private readonly AppOptions _appOptions;
    private readonly IUserContextService _userContext;
    
    public EmailTemplate StrategyKey => EmailTemplate.UserAccountActivation;

    public UserAccountActivationEmailStrategy(
        IEmailSender emailSender, 
        IOptions<AppOptions> options,
        IUserContextService userContext)
    {
        _emailSender = emailSender;
        _appOptions = options.Value;
        _userContext = userContext;
    }

    public async Task ExecuteAsync(string toEmail, IDictionary<string, string> data)
    {
        var link = UrlBuilder.BuildApiUrl(
            _appOptions.BaseUrl,
            version: 1,
            path: _appOptions.EmailVerificationUrl,
            queryParams: new()
            {
                ["token"] = data?["token"],
                ["locale"] = data.GetOrDefault("locale", "en"),
                ["traceId"] = data?["traceId"],
                ["tenantId"] = data?["tenantId"],
                ["userId"] = data?["userId"],
                ["timestamp"] = data?["timestamp"]
            });


        var variables = new Dictionary<string, string> { ["activation_link"] = link! };

        await _emailSender.SendAsync(toEmail, EmailTemplate.UserAccountActivation, variables);
    }
}