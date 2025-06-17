using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Infrastructure.Common.DependencyInjection;
using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Messaging.Interfaces;
using SmartRetail360.Notifications.Services.Configuration;

namespace SmartRetail360.Infrastructure.Services.AccountRegistration.Models;

public class AccountRegistrationDependencies : BaseDependencies
{
    public EmailContext EmailContext { get; set; }
    public IEmailQueueProducer EmailQueueProducer { get; set; }
    public IAuditLogger AuditLogger { get; set; }
    public IAbacPolicyService AbacPolicyService { get; set; }
}