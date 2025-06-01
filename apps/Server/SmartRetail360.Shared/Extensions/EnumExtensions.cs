using System.Reflection;
using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Extensions;

public static class EnumExtensions
{
    public static string GetEnumMemberValue<T>(this T enumValue) where T : struct, Enum
    {
        var member = typeof(T).GetMember(enumValue.ToString()).FirstOrDefault();
        var attribute = member?.GetCustomAttribute<EnumMemberAttribute>();
        return attribute?.Value ?? enumValue.ToString();
    }

    public static T ToEnumFromMemberValue<T>(this string value) where T : struct, Enum
    {
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
            if (attribute?.Value == value)
                return (T)field.GetValue(null)!;
        }

        // fallback：直接匹配名称
        if (Enum.TryParse<T>(value, out var parsed))
            return parsed;

        throw new ArgumentException($"Invalid value '{value}' for enum type '{typeof(T).Name}'");
    }
}