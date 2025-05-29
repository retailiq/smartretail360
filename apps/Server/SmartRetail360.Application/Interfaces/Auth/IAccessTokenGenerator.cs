namespace SmartRetail360.Application.Interfaces.Auth;

public interface IAccessTokenGenerator
{
    string GenerateToken(
        string userId,
        string email,
        string name,
        string tenantId,
        string roleId,
        string locale,
        string traceId);
}