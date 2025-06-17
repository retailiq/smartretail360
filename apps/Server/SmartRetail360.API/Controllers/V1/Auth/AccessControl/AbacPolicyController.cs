using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Logic;
using Microsoft.AspNetCore.Mvc;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Shared.Extensions;

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

    [HttpPut("{policyId:guid}/edit/rule")]
    public async Task<IActionResult> UpdatePolicyRuleJson(Guid policyId,
        [FromBody] UpdateAbacPolicyRuleJsonRequest ruleJsonRequest)
    {
        var ruleResult = await _abacPolicyService.UpdatePolicyRuleJsonAsync(policyId, ruleJsonRequest);
        return ruleResult.ToHttpResult();
    }

    [HttpPut("{policyId:guid}/edit/status")]
    public async Task<IActionResult> UpdatePolicyStatus(Guid policyId, [FromBody] UpdateAbacPolicyStatusRequest request)
    {
        var result = await _abacPolicyService.UpdatePolicyStatusAsync(policyId, request);
        return result.ToHttpResult();
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