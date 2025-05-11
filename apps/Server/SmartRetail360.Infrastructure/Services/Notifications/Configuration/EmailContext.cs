using SmartRetail360.Application.Interfaces.Notifications.Strategies;
using SmartRetail360.Domain.Entities;

namespace SmartRetail360.Infrastructure.Services.Notifications.Configuration;

public class EmailContext
{
    private IEmailStrategy? _strategy;

    public void SetStrategy(IEmailStrategy strategy)
    {
        _strategy = strategy;
    }

    public async Task ExecuteAsync(Tenant tenant)
    {
        if (_strategy == null) throw new InvalidOperationException("Email strategy not set.");
        await _strategy.ExecuteAsync(tenant);
    }
}