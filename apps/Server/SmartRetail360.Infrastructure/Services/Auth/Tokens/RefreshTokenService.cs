using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Models;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Services.Auth.Login.Interfaces;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Auth.Tokens;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _db;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IGuardChecker _guardChecker;

    public RefreshTokenService(
        AppDbContext db,
        ISafeExecutor safeExecutor,
        IGuardChecker guardChecker)
    {
        _db = db;
        _safeExecutor = safeExecutor;
        _guardChecker = guardChecker;
    }

    public async Task<string> CreateRefreshTokenAsync(RefreshTokenCreationContext ctx)
    {
        var token = TokenHelper.GenerateRefreshToken();

        var entity = new RefreshToken
        {
            UserId = ctx.UserId,
            TenantId = ctx.TenantId,
            RoleId = ctx.RoleId,
            Email = ctx.Email,
            Name = ctx.Name,
            Locale = ctx.Locale,
            TraceId = ctx.TraceId,
            Token = TokenHelper.HashToken(token),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ctx.IpAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(ctx.ExpiryDays)
        };

        await _safeExecutor.ExecuteAsync(
            async () =>
            {
                await _db.RefreshTokens.AddAsync(entity);
                await _db.SaveChangesAsync();
            },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );

        return token;
    }

    // ✅ Refresh Token Rotation
    public async Task<(string? Token, RefreshToken? Entity)> RotateRefreshTokenAsync(string oldToken, string ipAddress, int expiryDays)
    {
        var oldHash = TokenHelper.HashToken(oldToken);

        var tokenEntityResult = await _safeExecutor.ExecuteAsync(
            () =>
                _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == oldHash),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        var tokenEntity = tokenEntityResult.Response.Data;

        await _guardChecker
            .Check(() => tokenEntity is { ReplacedByToken: not null },
                LogEventType.RefreshTokenReplayDetected, LogReasons.RefreshTokenReplayDetected,
                ErrorCodes.None)
            .ValidateAsync();

        if (tokenEntity is not { IsActive: true }) return (null, null);

        // Generate new token and hash it
        var newToken = TokenHelper.GenerateRefreshToken();
        var newHash = TokenHelper.HashToken(newToken);

        var newEntity = new RefreshToken
        {
            UserId = tokenEntity.UserId,
            TenantId = tokenEntity.TenantId,
            Token = newHash,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            RoleId = tokenEntity.RoleId,
            Email = tokenEntity.Email,
            Name = tokenEntity.Name,
            Locale = tokenEntity.Locale,
            TraceId = tokenEntity.TraceId,
        };
        
        // Revoke the old token
        tokenEntity.RevokedAt = DateTime.UtcNow;
        tokenEntity.RevokedByIp = ipAddress;
        tokenEntity.ReplacedByToken = newHash;
        tokenEntity.ReasonRevokedEnum = RefreshTokenRevokeReason.TokenRotation;

        await _safeExecutor.ExecuteAsync(
            async () =>
            {
                await _db.RefreshTokens.AddAsync(newEntity);
                await _db.SaveChangesAsync();
            },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );

        return (newToken, newEntity);
    }

    // Logout and Revoke Token
    public async Task<bool> RevokeRefreshTokenAsync(string token, string ipAddress, RefreshTokenRevokeReason reason)
    {
        var hash = TokenHelper.HashToken(token);
        var entityResult = await _safeExecutor.ExecuteAsync(
            () =>
                _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == hash),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        var entity = entityResult.Response.Data;

        var checkResult = await _guardChecker
            .Check(() => entity is not { RevokedAt : null },
                LogEventType.LogoutFailed, LogReasons.RefreshTokenNotFound,
                ErrorCodes.None)
            .ValidateAsync();
        if (checkResult != null)
            return false;

        entity!.RevokedAt = DateTime.UtcNow;
        entity.RevokedByIp = ipAddress;
        entity.ReasonRevokedEnum = reason;
        entity.ReplacedByToken = hash;

        await _safeExecutor.ExecuteAsync(
            async () => { await _db.SaveChangesAsync(); },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );

        return true;
    }
}