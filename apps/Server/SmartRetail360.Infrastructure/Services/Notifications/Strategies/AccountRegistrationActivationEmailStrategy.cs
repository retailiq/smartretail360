using System.Diagnostics.CodeAnalysis;
using SmartRetail360.Application.Interfaces.Notifications.Strategies;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Utils;
using Microsoft.Extensions.Options;
using SmartRetail360.Application.Interfaces.Notifications.Configuration;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Infrastructure.Services.Notifications.Strategies;

public class AccountRegistrationActivationEmailStrategy : IEmailStrategy
{
    private readonly IEmailSender _emailSender;
    private readonly AppOptions _appOptions;
    
    public EmailTemplate StrategyKey => EmailTemplate.AccountRegistrationActivation;
    
    public AccountRegistrationActivationEmailStrategy(
        IEmailSender emailSender, 
        IOptions<AppOptions> options)
    {
        _emailSender = emailSender;
        _appOptions = options.Value;
    }
    
    public async Task ExecuteAsync(string toEmail, IDictionary<string, string> data)
    {
        var link = UrlBuilder.BuildApiUrl(
            _appOptions.BaseUrl,
            version: 1,
            path: _appOptions.EmailVerificationUrl,
            queryParams: new()
            {
                ["token"] = data["token"],
                ["locale"] = data.GetOrDefault("locale", "en"),
                ["traceId"] = data["traceId"],
                ["timestamp"] = data["timestamp"]
            });

        var variables = new Dictionary<string, string>(data) { ["activation_link"] = link };

        await _emailSender.SendAsync(toEmail, EmailTemplate.AccountRegistrationActivation, variables);
    }
}