using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Infrastructure.Services.Auth.Login.Interfaces;
using SmartRetail360.Infrastructure.Services.Auth.Models;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.CredentialsLogin;

public class CredentialsLoginContext : LoginContextBase<LoginRequest>
{
    public CredentialsLoginContext(LoginDependencies dep, LoginRequest request)
        : base(dep, request)
    {
    }
}