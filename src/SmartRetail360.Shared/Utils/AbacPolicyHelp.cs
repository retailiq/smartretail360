using System.Text.Json.Nodes;

namespace SmartRetail360.Shared.Utils;

public static class AbacPolicyHelp
{
    public static bool IsValidRule(string json)
    {
        try
        {
            var rule = JsonNode.Parse(json);
            return rule switch
            {
                JsonValue value when value.TryGetValue<bool>(out _) => false,
                JsonObject { Count: > 0 } obj => obj.FirstOrDefault() is var (key, valueNode) && key switch
                {
                    "==" or "===" when valueNode is JsonArray { Count: 2 } => true,
                    "and" when valueNode is JsonArray andArray && andArray.All(x => x is JsonObject) => true,
                    _ => false
                },
                _ => false
            };
        }
        catch
        {
            return false;
        }
    }
}