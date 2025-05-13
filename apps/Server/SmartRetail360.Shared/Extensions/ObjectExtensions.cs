using System.Text.Json;

namespace SmartRetail360.Shared.Extensions;

public static class ObjectExtensions
{
    public static Dictionary<string, string> ToDictionary(this object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
    }
}