using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Caching;

public interface IRoleCacheService
{
    Task<Role?> GetSystemRoleAsync(SystemRoleType roleType);
    Task<List<Role>> GetSystemRolesByIdsAsync(List<Guid> roleIds);
}