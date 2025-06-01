using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Logic;
using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Interfaces.Auth.AccessControl;

namespace SmartRetail360.Infrastructure.Services.Auth.AccessControl;

public class JsonLogicPolicyEvaluator : IPolicyEvaluator
{
    private readonly ILogger<JsonLogicPolicyEvaluator> _logger;
    private readonly IPolicyRepo _policyRepo;

    public JsonLogicPolicyEvaluator(
        ILogger<JsonLogicPolicyEvaluator> logger,
        IPolicyRepo policyRepo)
    {
        _logger = logger;
        _policyRepo = policyRepo;
    }

    public async Task<bool> EvaluateAsync(Guid tenantId, string resourceType, string action, object context)
    {
        var envElement = JsonSerializer.SerializeToElement(context);
        var envName = envElement.GetProperty("environment").GetProperty("name").GetString() ?? "default";
        var ruleJson = await _policyRepo.GetPolicyJsonAsync(tenantId, resourceType, action, envName);

        if (string.IsNullOrWhiteSpace(ruleJson))
        {
            _logger.LogWarning("[ABAC] No policy found for {Resource}:{Action}", resourceType, action);
            return false;
        }

        try
        {
            var ruleNode = JsonNode.Parse(ruleJson);
            var dataNode = JsonSerializer.SerializeToNode(context);

            var result = JsonLogic.Apply(ruleNode!, dataNode!);
            var isAllowed = result?.GetValue<bool>() ?? false;

            _logger.LogInformation("[ABAC] Evaluated policy {Resource}:{Action} => {Result}", resourceType, action,
                isAllowed);
            return isAllowed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ABAC] Failed to evaluate policy for {Resource}:{Action}", resourceType, action);
            return false;
        }
    }
}