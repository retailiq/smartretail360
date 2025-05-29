using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Application.Interfaces.Messaging;
using SmartRetail360.Infrastructure.Common.DependencyInjection;
using SmartRetail360.Infrastructure.Services.Notifications.Configuration;

namespace SmartRetail360.Infrastructure.Services.AccountRegistration.Models;

public class AccountRegistrationDependencies : BaseDependencies
{
    public EmailContext EmailContext { get; set; }
    public IEmailQueueProducer EmailQueueProducer { get; set; }
    public IAuditLogger AuditLogger { get; set; }
}