using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Interfaces.Auth.AccessControl;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Infrastructure.Common.DependencyInjection;

namespace SmartRetail360.Infrastructure.Services.Auth.Models;

public class LoginDependencies : BaseDependencies
{
    public IAccessTokenGenerator AccessTokenGenerator { get; set; }
    public IAccountSupportService AccountSupport { get; set; }
    public IAbacPolicyService AbacPolicyService { get; set; }
}