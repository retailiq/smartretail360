using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Notifications.Strategies;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Options;
using SmartRetail360.Shared.Utils;
using Microsoft.Extensions.Options;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Shared.Localization;

namespace SmartRetail360.Infrastructure.Services.Notifications.Strategies;

public class AccountActivationEmailStrategy : IEmailStrategy
{
    private readonly IEmailSender _emailSender;
    private readonly AppOptions _appOptions;
    private readonly IUserContextService _userContext;

    public AccountActivationEmailStrategy(
        IEmailSender emailSender, 
        IOptions<AppOptions> options,
        IUserContextService userContext)
    {
        _emailSender = emailSender;
        _appOptions = options.Value;
        _userContext = userContext;
    }

    public async Task ExecuteAsync(Tenant tenant)
    {
        var link = UrlBuilder.BuildApiUrl(
            _appOptions.BaseUrl,
            version: 1,
            path: _appOptions.EmailVerificationUrl,
            queryParams: new()
            {
                ["token"] = tenant.EmailVerificationToken!,
                ["locale"] = _userContext.Locale ?? "en",
                ["traceId"] = _userContext.TraceId ?? string.Empty,
                ["tenantId"] = tenant.Id.ToString(),
                ["timestamp"] = DateTime.UtcNow.ToString("o")
            });

        var variables = new Dictionary<string, string> { ["activation_link"] = link! };

        await _emailSender.SendAsync(tenant.AdminEmail, EmailTemplate.AccountActivation, variables);
    }
}