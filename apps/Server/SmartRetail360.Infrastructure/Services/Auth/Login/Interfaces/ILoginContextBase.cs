using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.Interfaces;

public interface ILoginContextBase
{
    User? User { get; }
    List<TenantUser>? TenantUsers { get; set; }
    List<Role>? Roles { get; set; }
    LoginDependencies Dep { get; }
    string TraceId { get; }
}