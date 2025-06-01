using SmartRetail360.Domain.Entities.AccessControl;

namespace SmartRetail360.Application.Interfaces.Auth.AccessControl;

public interface IPolicyRepo
{
    Task<string?> GetPolicyJsonAsync(Guid tenantId,string resourceType, string action, string environmentName);
    Task<Dictionary<string, Guid>> GetAllResourceTypeMapAsync();
    Task<Dictionary<string, Guid>> GetAllActionMapAsync();
    Task<Dictionary<string, Guid>> GetAllEnvironmentMapAsync();
    Task<List<string>> GetGroupResourcesAsync(string groupName);
    Task<List<AbacPolicyTemplate>> GetPolicyTemplatesAsync();
    Task<Dictionary<string, List<string>>> GetAllResourceGroupMapAsync();
}