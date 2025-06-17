using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums.AccessControl;

public enum DefaultEnvironmentType
{
    [EnumMember(Value = "default")]
    Default,
    
    [EnumMember(Value = "client")]
    Client,

    [EnumMember(Value = "admin")]
    Admin,
    
    [EnumMember(Value = "system")]
    System,
}