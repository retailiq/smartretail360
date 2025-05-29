using SmartRetail360.Application.Models;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Auth;

public interface IRefreshTokenService
{
    Task<string> CreateRefreshTokenAsync(RefreshTokenCreationContext ctx);
    Task<(string? Token, RefreshToken? Entity)> RotateRefreshTokenAsync(string oldToken, string ipAddress, int expiryDays);
    Task<bool> RevokeRefreshTokenAsync(string token, string ipAddress, RefreshTokenRevokeReason reason);
}