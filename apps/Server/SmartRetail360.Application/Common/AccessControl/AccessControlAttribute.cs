namespace SmartRetail360.Application.Common.AccessControl;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AccessControlAttribute : Attribute
{
    public string ResourceType { get; }
    public string Action { get; }

    public AccessControlAttribute(string resourceType, string action)
    {
        ResourceType = resourceType;
        Action = action;
    }
}