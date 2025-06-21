using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartRetail360.Shared.Json;

public class JsonEnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsEnum || (Nullable.GetUnderlyingType(typeToConvert)?.IsEnum ?? false);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var isNullable = Nullable.GetUnderlyingType(typeToConvert) != null;
        var enumType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;

        if (!enumType.IsEnum)
            throw new InvalidOperationException($"Type {enumType} is not an enum.");

        var converterType = isNullable
            ? typeof(EnumMemberNullableJsonConverter<>).MakeGenericType(enumType)
            : typeof(EnumMemberJsonConverter<>).MakeGenericType(enumType);

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}