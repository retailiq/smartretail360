using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.Auth.Models;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.Interfaces;

public abstract class LoginContextBase<TRequest> : ILoginContextBase
{
    public LoginDependencies Dep { get; }
    public TRequest Request { get; }
    public string TraceId => Dep.UserContext.TraceId;

    public User? User { get; set; }
    public List<TenantUser>? TenantUsers { get; set; }
    public List<Role>? Roles { get; set; }
    
    protected LoginContextBase(LoginDependencies dep, TRequest request)
    {
        Dep = dep;
        Request = request;
    }
}