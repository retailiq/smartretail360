using SmartRetail360.Application.Models;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Interfaces.Auth;

public interface IRefreshTokenService
{
    Task<string?> CreateRefreshTokenAsync(RefreshTokenCreationContext ctx);
    Task<string?> RotateRefreshTokenAsync(RefreshToken oldEntity, string ipAddress);
    Task<bool> RevokeRefreshTokenAsync(string token, string ipAddress, RefreshTokenRevokeReason reason);
}