using SmartRetail360.Domain.Entities.AccessControl;

namespace SmartRetail360.ABAC.Interfaces;

public interface IPolicyRepo
{
    Task<string?> GetPolicyJsonAsync(Guid tenantId,string resourceType, string action);
    Task<Dictionary<string, Guid>> GetAllResourceTypeMapAsync();
    Task<Dictionary<string, Guid>> GetAllActionMapAsync();
    Task<Dictionary<string, Guid>> GetAllEnvironmentMapAsync();
    Task<List<AbacPolicyTemplate>> GetPolicyTemplatesMapAsync();
    Task<Dictionary<string, List<string>>> GetAllResourceGroupMapAsync();
}