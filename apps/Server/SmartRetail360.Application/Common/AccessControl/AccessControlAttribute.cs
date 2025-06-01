using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Application.Common.AccessControl;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AccessControlAttribute : Attribute
{
    public string ResourceType { get; }
    public string Action { get; }

    public AccessControlAttribute(DefaultResourceType resource, DefaultActionType action)
    {
        ResourceType = resource.GetEnumMemberValue();
        Action = action.GetEnumMemberValue();
    }
}