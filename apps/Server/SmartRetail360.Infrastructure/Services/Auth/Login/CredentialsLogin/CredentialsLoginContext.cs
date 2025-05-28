using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Infrastructure.Services.Auth.Login.Shared;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Redis;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.CredentialsLogin;

public class CredentialsLoginContext : LoginContextBase<LoginRequest>
{
    public CredentialsLoginContext(LoginDependencies dep, LoginRequest request)
        : base(dep, request)
    {
        LockKey = RedisKeys.UserLoginLock(request.Email.ToLower());
        FailKey = RedisKeys.UserLoginFailures(request.Email);
        SecurityKey = RedisKeys.UserLoginSecurityLock(request.Email);
    }
}