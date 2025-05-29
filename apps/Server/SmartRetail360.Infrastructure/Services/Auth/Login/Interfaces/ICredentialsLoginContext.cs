namespace SmartRetail360.Infrastructure.Services.Auth.Login.Interfaces;

public interface ICredentialsLoginContext : ILoginContextBase
{
    string LockKey { get; }
    string FailKey { get; }
    string SecurityKey { get; }
}