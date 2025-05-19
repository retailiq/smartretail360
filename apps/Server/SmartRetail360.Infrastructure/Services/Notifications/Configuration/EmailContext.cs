using SmartRetail360.Application.Interfaces.Notifications.Strategies;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Infrastructure.Services.Notifications.Configuration;

public class EmailContext
{
    private readonly Dictionary<EmailTemplate, IEmailStrategy> _strategies;

    public EmailContext(IEnumerable<IEmailStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.StrategyKey);
    }

    public Task SendAsync(EmailTemplate template, string toEmail, IDictionary<string, string> variables)
    {
        if (!_strategies.TryGetValue(template, out var strategy))
            throw new InvalidOperationException($"No strategy found for template: {template}");
        return strategy.ExecuteAsync(toEmail, variables);
    }
}