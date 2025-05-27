using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Redis;

namespace SmartRetail360.Infrastructure.Services.Auth.UserLogin;

public class LoginContext
{
    public LoginDependencies _dep;
    public LoginRequest Request;
    public string LockKey;
    public string FailKey;
    public string SecurityKey;
    public string TraceId => _dep.UserContext.TraceId;

    public Domain.Entities.User? User;
    public List<TenantUser>? TenantUsers;
    public List<Role>? Roles;

    public LoginContext(LoginDependencies dep, LoginRequest request)
    {
        _dep = dep;
        Request = request;
        LockKey = RedisKeys.UserLoginLock(request.Email.ToLower());
        FailKey = RedisKeys.UserLoginFailures(request.Email);
        SecurityKey = RedisKeys.UserLoginSecurityLock(request.Email);
    }
}