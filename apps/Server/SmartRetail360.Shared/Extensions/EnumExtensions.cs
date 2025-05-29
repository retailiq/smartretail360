using System.Reflection;
using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Extensions;

public static class EnumExtensions
{
    public static string GetEnumMemberValue<T>(this T enumValue) where T : struct, Enum =>
        typeof(T).GetMember(enumValue.ToString())[0]
            .GetCustomAttribute<EnumMemberAttribute>()?.Value ?? enumValue.ToString();

    public static T ToEnumFromMemberValue<T>(this string value) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return Enum.Parse<T>("En"); 
        
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
            if (attribute?.Value == value)
                return Enum.Parse<T>(field.Name);
        }

        throw new ArgumentException($"Invalid value '{value}' for enum type '{typeof(T).Name}'");
    }
}