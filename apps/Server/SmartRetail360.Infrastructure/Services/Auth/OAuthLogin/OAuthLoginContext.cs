using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.Auth.Models;

namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin;

public class OAuthLoginContext
{
    public OAuthLoginRequest Request;
    public OAuthLoginDependencies _dep;
    public string TraceId => _dep.UserContext.TraceId;

    public Domain.Entities.User? User;
    public List<TenantUser>? TenantUsers;
    public List<Role>? Roles;

    public OAuthLoginContext(OAuthLoginDependencies dep, OAuthLoginRequest request)
    {
        _dep = dep;
        Request = request;
    }
}