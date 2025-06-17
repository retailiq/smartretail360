using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Notifications.Interfaces.Strategies;

public interface IEmailStrategy
{
    EmailTemplate StrategyKey { get; }
    Task ExecuteAsync(string toEmail, IDictionary<string, string> data);
}