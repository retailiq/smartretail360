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
    
    [EnumMember(Value = "send")]
    Send,
    
    [EnumMember(Value = "remove")]
    Remove,
    
    [EnumMember(Value = "leave")]
    Leave,
    
    [EnumMember(Value = "suspend")]
    Suspend,
    
    [EnumMember(Value = "unsuspend")]
    Unsuspend,
    
    [EnumMember(Value = "invite")]
    Invite,
    
    [EnumMember(Value = "audit")]
    Audit,
    
    // only for tenant deactivate the user account belonging to the tenant
    [EnumMember(Value = "tenant_deactivate")]
    TenantDeactivate,
    
    // only for tenant reactivate the user account belonging to the tenant
    [EnumMember(Value = "tenant_reactivate")]
    TenantReactivate,
}