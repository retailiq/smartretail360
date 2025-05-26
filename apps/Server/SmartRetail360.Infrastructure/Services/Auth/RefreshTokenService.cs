using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _db;

    public RefreshTokenService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string> CreateRefreshTokenAsync(Guid userId, Guid tenantId, string ipAddress, int expiryDays)
    {
        var token = TokenHelper.GenerateRefreshToken();
        
        var entity = new RefreshToken
        {
            UserId = userId,
            TenantId = tenantId,
            Token = TokenHelper.HashToken(token),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };

        await _db.RefreshTokens.AddAsync(entity);
        await _db.SaveChangesAsync();
        return token;
    }
    
    // ✅ Refresh Token Rotation
    public async Task<string?> RotateRefreshTokenAsync(string oldToken, string ipAddress, int expiryDays)
    {
        var oldHash = TokenHelper.HashToken(oldToken);
        var tokenEntity = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == oldHash);

        if (tokenEntity is not { IsActive: true }) return null;

        // 生成新 token
        var newToken = TokenHelper.GenerateRefreshToken();
        var newHash = TokenHelper.HashToken(newToken);

        var newEntity = new RefreshToken
        {
            UserId = tokenEntity.UserId,
            TenantId = tokenEntity.TenantId,
            Token = newHash,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };

        // 标记旧 token 为 revoked
        tokenEntity.RevokedAt = DateTime.UtcNow;
        tokenEntity.RevokedByIp = ipAddress;
        tokenEntity.ReplacedByToken = newHash;
        tokenEntity.ReasonRevokedEnum = RefreshTokenRevokeReason.TokenRotation;

        await _db.RefreshTokens.AddAsync(newEntity);
        await _db.SaveChangesAsync();

        return newToken;
    }
    
    // ✅ 手动 Revoke（登出）
    public async Task<bool> RevokeRefreshTokenAsync(string token, string ipAddress, RefreshTokenRevokeReason reason)
    {
        var hash = TokenHelper.HashToken(token);
        var entity = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == hash);
        if (entity == null || entity.RevokedAt != null) return false;

        entity.RevokedAt = DateTime.UtcNow;
        entity.RevokedByIp = ipAddress;
        entity.ReasonRevokedEnum = reason;

        await _db.SaveChangesAsync();
        return true;
    }

}