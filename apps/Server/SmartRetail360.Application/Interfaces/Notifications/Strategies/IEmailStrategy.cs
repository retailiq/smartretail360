using SmartRetail360.Domain.Entities;

namespace SmartRetail360.Application.Interfaces.Notifications.Strategies;

public interface IEmailStrategy
{
    Task ExecuteAsync(Tenant tenant);
}
