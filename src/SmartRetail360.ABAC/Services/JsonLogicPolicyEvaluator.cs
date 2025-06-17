using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Logic;
using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.Execution;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.ABAC.Services;

public class JsonLogicPolicyEvaluator : IPolicyEvaluator
{
    private readonly IPolicyRepo _policyRepo;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IGuardChecker _guardChecker;

    public JsonLogicPolicyEvaluator(
        IPolicyRepo policyRepo,
        ISafeExecutor safeExecutor,
        IGuardChecker guardChecker)
    {
        _policyRepo = policyRepo;
        _safeExecutor = safeExecutor;
        _guardChecker = guardChecker;
    }

    public async Task<bool> EvaluateAsync(Guid tenantId, string resourceType, string action, object context)
    {
        var envElement = JsonSerializer.SerializeToElement(context);

        Console.WriteLine("[ABAC in JsonPolicyEvaluator] Env Element: " + envElement.ToString());

        var ruleJson = await _policyRepo.GetPolicyJsonAsync(tenantId, resourceType, action);
        
        var checkResult = await _guardChecker
            .Check(() => string.IsNullOrWhiteSpace(ruleJson),
                LogEventType.AbacPolicyEvaluationFailure, LogReasons.PolicyNotFoundOrDisabled,
                ErrorCodes.None)
            .ValidateAsync();
        if (checkResult != null)
            return false;

        var evalResult = await _safeExecutor.ExecuteAsync(
            () =>
            {
                var ruleNode = JsonNode.Parse(ruleJson!);
                var dataNode = JsonSerializer.SerializeToNode(context);
                var result = JsonLogic.Apply(ruleNode!, dataNode!);
                return Task.FromResult(result?.GetValue<bool>() ?? false);
            },
            LogEventType.AbacPolicyEvaluationFailure,
            LogReasons.PolicyEvaluationFailed,
            ErrorCodes.None
        );

        return evalResult.IsSuccess && evalResult.Response.Data;
    }
}