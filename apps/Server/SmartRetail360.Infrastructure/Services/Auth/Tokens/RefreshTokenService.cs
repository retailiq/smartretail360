using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Models;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Execution;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using SmartRetail360.Persistence.Data;

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

    public async Task<string?> CreateRefreshTokenAsync(RefreshTokenCreationContext ctx)
    {
        var token = TokenHelper.GenerateRefreshToken();

        var entity = new RefreshToken
        {
            UserId = ctx.UserId,
            TenantId = ctx.TenantId,
            RoleId = ctx.RoleId,
            Email = ctx.Email,
            UserName = ctx.UserName,
            Locale = ctx.Locale,
            TraceId = ctx.TraceId,
            Token = TokenHelper.HashToken(token),
            Env = ctx.Env,
            RoleName = ctx.RoleName,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ctx.IpAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(ctx.ExpiryDays)
        };

        var saveResult = await _safeExecutor.ExecuteAsync(
            async () =>
            {
                await _db.RefreshTokens.AddAsync(entity);
                await _db.SaveChangesAsync();
            },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );
        if (!saveResult.IsSuccess) return null;

        return token;
    }

    // ✅ Refresh Token Rotation
    public async Task<string?> RotateRefreshTokenAsync(RefreshToken oldEntity, string ipAddress)
    {
        var remaining = oldEntity.ExpiresAt - DateTime.UtcNow;
        if (remaining <= TimeSpan.Zero) return null;

        // Generate new token and hash it
        var newToken = TokenHelper.GenerateRefreshToken();
        var newHash = TokenHelper.HashToken(newToken);

        oldEntity.RevokedAt = DateTime.UtcNow;
        oldEntity.RevokedByIp = ipAddress;
        oldEntity.ReplacedByToken = newHash;
        oldEntity.ReasonRevokedEnum = RefreshTokenRevokeReason.TokenRotation;

        var newEntity = new RefreshToken
        {
            UserId = oldEntity.UserId,
            TenantId = oldEntity.TenantId,
            RoleId = oldEntity.RoleId,
            Email = oldEntity.Email,
            UserName = oldEntity.UserName,
            Locale = oldEntity.Locale,
            TraceId = oldEntity.TraceId,
            Token = newHash,
            Env = oldEntity.Env,
            CreatedByIp = ipAddress,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(remaining)
        };

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

        return newToken;
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
                LogEventType.LogoutFailed, LogReasons.RefreshTokenMissing,
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