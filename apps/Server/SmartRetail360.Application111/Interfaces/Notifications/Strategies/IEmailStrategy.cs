using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Notifications.Strategies;

public interface IEmailStrategy
{
    EmailTemplate StrategyKey { get; }
    Task ExecuteAsync(string toEmail, IDictionary<string, string> data);
}