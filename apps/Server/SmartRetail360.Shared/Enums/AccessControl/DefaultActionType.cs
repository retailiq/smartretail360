using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums.AccessControl;

public enum DefaultActionType
{
    [EnumMember(Value = "none")]
    None,
    
    [EnumMember(Value = "view")]
    View,

    [EnumMember(Value = "edit")]
    Edit,

    [EnumMember(Value = "delete")]
    Delete,
    
    [EnumMember(Value = "create")]
    Create,
}