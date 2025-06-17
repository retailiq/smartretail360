namespace SmartRetail360.ABAC.Interfaces;

public interface IAbacRouteMapper
{
    (string resourceType, string action)? Resolve(string path);
    bool IsProtected(string path);
}