using SmartRetail360.Application.Models;

namespace SmartRetail360.Application.Interfaces.Auth;

public interface IAccessTokenGenerator
{
    string GenerateToken(AccessTokenCreationContext payload);
}