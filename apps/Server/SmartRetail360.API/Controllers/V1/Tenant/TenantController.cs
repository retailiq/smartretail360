using Microsoft.AspNetCore.Mvc;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.API.Controllers.V1.Tenant;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/tenants")]
public class TenantController : ControllerBase
{
    private readonly IAbacPolicyService _abacPolicyService;

    public TenantController(IAbacPolicyService abacPolicyService)
    {
        _abacPolicyService = abacPolicyService;
    }

    [HttpGet("{tenantId:guid}/abac-policies/view")]
    public async Task<IActionResult> GetPoliciesForTenant(Guid tenantId)
    {
        var result = await _abacPolicyService.GetAllPoliciesForTenantAsync(tenantId);
        return result.ToHttpResult();
    }
}