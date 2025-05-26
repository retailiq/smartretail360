namespace SmartRetail360.Application.Interfaces.Auth;

public interface IRefreshTokenService
{
    Task<string> CreateRefreshTokenAsync(Guid userId, Guid tenantId, string ipAddress, int expiryDays);
}