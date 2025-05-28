using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.Auth.Models;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.Interfaces;

public interface ILoginContextBase
{
    User? User { get; }
    List<TenantUser>? TenantUsers { get; set; }
    List<Role>? Roles { get; set; }
    LoginDependencies Dep { get; }
    string FailKey { get; }
    string SecurityKey { get; }
    string TraceId { get; }
    string? LockKey { get; }
}