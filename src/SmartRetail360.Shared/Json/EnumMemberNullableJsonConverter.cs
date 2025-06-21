using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartRetail360.Shared.Json;

public class EnumMemberNullableJsonConverter<T> : JsonConverter<T?> where T : struct, Enum
{
    private readonly Dictionary<string, T> _fromValue;
    private readonly Dictionary<T, string> _toValue;

    public EnumMemberNullableJsonConverter()
    {
        _fromValue = new(StringComparer.OrdinalIgnoreCase);
        _toValue = new();

        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var enumValue = (T)field.GetValue(null)!;
            var enumMemberAttr = field.GetCustomAttribute<EnumMemberAttribute>();
            var enumString = enumMemberAttr?.Value ?? field.Name;

            _fromValue[enumString] = enumValue;
            _toValue[enumValue] = enumString;
        }
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var enumText = reader.GetString();
        if (enumText != null && _fromValue.TryGetValue(enumText, out var value))
            return value;

        throw new JsonException($"Unable to convert \"{enumText}\" to enum {typeof(T)}.");
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value.HasValue && _toValue.TryGetValue(value.Value, out var enumString))
            writer.WriteStringValue(enumString);
        else
            writer.WriteNullValue();
    }
}