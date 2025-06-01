using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Logic;
using Microsoft.AspNetCore.Mvc;
using SmartRetail360.Application.Interfaces.Auth.AccessControl;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.API.Controllers.V1.Auth.AccessControl;

[ApiController]
[Route("api/v{version:apiVersion}/abac-policies")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class AbacPolicyController : ControllerBase
{
    private readonly IAbacPolicyService _abacPolicyService;

    public AbacPolicyController(IAbacPolicyService abacPolicyService)
    {
        _abacPolicyService = abacPolicyService;
    }

    [HttpGet("{tenantId:guid}")]
    public async Task<ActionResult<ApiResponse<List<AbacPolicy>>>> GetPolicies(Guid tenantId)
    {
        var policies = await _abacPolicyService.GetAllPoliciesForTenantAsync(tenantId);
        return Ok(policies);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePolicy(Guid id,
        [FromBody] UpdateAbacPolicyRequest request)
    {
        if (id != request.Id)
            return BadRequest("ID mismatch.");

        var result = await _abacPolicyService.UpdatePolicyAsync(request);
        return Ok(result);
    }

    [HttpPost("evaluate-preview")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public IActionResult PreviewPolicyEvaluation([FromBody] AbacPolicyPreviewRequest request)
    {
        try
        {
            var dataNode = JsonSerializer.SerializeToNode(request.Context);
            var ruleNode = JsonNode.Parse(request.RuleJson);
            var result = JsonLogic.Apply(ruleNode!, dataNode!);
            return Ok(result?.GetValue<bool>() ?? false);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ABAC] PreviewPolicyEvaluation failed. Invalid input: " + ex.Message);
            return BadRequest("Invalid rule or context format.");
        }
    }
}