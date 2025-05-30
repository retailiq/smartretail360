using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Logic;
using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Interfaces.Auth.AccessControl;

namespace SmartRetail360.Infrastructure.Services.Auth.AccessControl;

public class JsonLogicPolicyEvaluator : IPolicyEvaluator
{
    private readonly ILogger<JsonLogicPolicyEvaluator> _logger;

    public JsonLogicPolicyEvaluator(ILogger<JsonLogicPolicyEvaluator> logger)
    {
        _logger = logger;
    }

    public async Task<bool> EvaluateAsync(string resourceType, string action, object context)
    {
        // 示例策略规则，可根据实际需求从数据库或配置文件加载
        var ruleJson = """
                       {
                           "==": [ { "var": "user.tenant_id" }, "tenant-001" ]
                       }
                       """;

        // 将规则和上下文数据解析为 JsonNode
        var ruleNode = JsonNode.Parse(ruleJson);
        var dataNode = JsonSerializer.SerializeToNode(context);

        // 应用规则
        var result = JsonLogic.Apply(ruleNode!, dataNode!);

        var isAllowed = result?.GetValue<bool>() ?? false;

        _logger.LogInformation("[ABAC] Evaluated policy for resource '{Resource}' and action '{Action}', result: {Result}",
            resourceType, action, isAllowed);

        return isAllowed;
    }
}