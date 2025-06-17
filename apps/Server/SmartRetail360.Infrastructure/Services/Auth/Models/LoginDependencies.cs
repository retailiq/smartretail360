using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Infrastructure.Common.DependencyInjection;
using SmartRetail360.Platform.Interfaces;

namespace SmartRetail360.Infrastructure.Services.Auth.Models;

public class LoginDependencies : BaseDependencies
{
    public IAccessTokenGenerator AccessTokenGenerator { get; set; }
    public IAccountSupportService AccountSupport { get; set; }
    public IAbacPolicyService AbacPolicyService { get; set; }
}