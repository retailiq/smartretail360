using Microsoft.Extensions.Options;
using SmartRetail360.Notifications.Interfaces.Configuration;
using SmartRetail360.Notifications.Interfaces.Strategies;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Notifications.Services.Strategies;

public class AccountRegistrationActivationEmailStrategy : IEmailStrategy
{
    private readonly IEmailSender _emailSender;
    private readonly AppOptions _appOptions;
    
    public EmailTemplate StrategyKey => EmailTemplate.UserRegistrationActivation;
    
    public AccountRegistrationActivationEmailStrategy(
        IEmailSender emailSender, 
        IOptions<AppOptions> options)
    {
        _emailSender = emailSender;
        _appOptions = options.Value;
    }
    
    public async Task ExecuteAsync(string toEmail, IDictionary<string, string> data)
    {
        foreach (var kv in data)
        {
            Console.WriteLine($"{kv.Key}: {kv.Value}");
            Console.WriteLine($"{kv.Key} (hex): {string.Join(" ", kv.Value.Select(c => ((int)c).ToString("X2")))}");
        }
        
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
        
        Console.WriteLine($"[DEBUG] Final activation link:\n{link}");

        var variables = new Dictionary<string, string>(data) { ["activation_link"] = link };

        Console.WriteLine("==== Final template variables ====");
        foreach (var kv in variables)
        {
            Console.WriteLine($"{kv.Key}: {kv.Value}");
        }
        
        await _emailSender.SendAsync(toEmail, EmailTemplate.UserRegistrationActivation, variables);
    }
}