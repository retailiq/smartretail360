using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartRetail360.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LocaleType
{
    [EnumMember(Value = "en")]
    En,

    [EnumMember(Value = "zh-CN")]
    ZhCn
}