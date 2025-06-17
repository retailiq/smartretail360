using System.Runtime.Serialization;

namespace SmartRetail360.Shared.Enums.AccessControl;

public enum DefaultResourceType
{
    [EnumMember(Value = "none")]
    None,
    
    [EnumMember(Value = "user")]
    User,

    [EnumMember(Value = "tenant")]
    Tenant,
    
    [EnumMember(Value = "role")]
    Role,

    [EnumMember(Value = "product")]
    Product,
    
    [EnumMember(Value = "order")]
    Order,
    
    [EnumMember(Value = "file")]
    File,

    [EnumMember(Value = "dataset")]
    Dataset,

    [EnumMember(Value = "report")]
    Report,
    
    [EnumMember(Value = "notification")]
    Notification,
    
    [EnumMember(Value = "dashboard")]
    Dashboard,

    [EnumMember(Value = "copilot_session")]
    CopilotSession,

    [EnumMember(Value = "recommendation")]
    Recommendation,

    [EnumMember(Value = "api_key")]
    ApiKey,

    [EnumMember(Value = "webhook")]
    Webhook,
    
    [EnumMember(Value = "abac-policy")]
    AbacPolicy,
}