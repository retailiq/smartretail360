using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Models;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Execution;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Users;

public class UpdateUserProfileTokenGenerator
{
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ISafeExecutor _executor;
    private readonly AppDbContext _dbContext;
    private readonly IGuardChecker _guardChecker;

    public UpdateUserProfileTokenGenerator(
        IAccessTokenGenerator accessTokenGenerator,
        IRefreshTokenService refreshTokenService,
        ISafeExecutor executor,
        AppDbContext dbContext,
        IGuardChecker guardChecker)
    {
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenService = refreshTokenService;
        _executor = executor;
        _dbContext = dbContext;
        _guardChecker = guardChecker;
    }

    public async Task<(string AccessToken, string? RefreshToken, DateTime? ExpiresAt)> GenerateTokensAsync(
        TenantUser tenantUser,
        string traceId,
        string env,
        string ipAddress,
        string oldRefreshToken)
    {
        var accessToken = _accessTokenGenerator.GenerateToken(new AccessTokenCreationContext
        {
            UserId = tenantUser.User!.Id.ToString(),
            Email = tenantUser.User!.Email,
            UserName = tenantUser.User!.Name,
            TenantId = tenantUser.TenantId.ToString(),
            RoleId = tenantUser.RoleId.ToString(),
            RoleName = tenantUser.Role!.Name,
            TraceId = traceId,
            Environment = env
        });
        
        var oldHash = TokenHelper.HashToken(oldRefreshToken);
        var tokenEntityResult = await _executor.ExecuteAsync(
            () =>
                _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == oldHash),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        var tokenEntity = tokenEntityResult.Response.Data;
        var validityCheckResult = await _guardChecker
            .Check(() => tokenEntity == null,
                LogEventType.RefreshTokenFailure, LogReasons.RefreshTokenMissing,
                ErrorCodes.RefreshTokenMissing)
            .Check(() => tokenEntity!.IsExpired,
                LogEventType.RefreshTokenFailure, LogReasons.RefreshTokenExpired,
                ErrorCodes.RefreshTokenExpired)
            .Check(() => tokenEntity!.IsRevoked,
                LogEventType.RefreshTokenFailure, LogReasons.RefreshTokenRevoked,
                ErrorCodes.RefreshTokenRevoked)
            .Check(() => tokenEntity!.ReplacedByToken != null,
                LogEventType.RefreshTokenReplayDetected, LogReasons.RefreshTokenReplayDetected,
                ErrorCodes.RefreshTokenReplayDetected)
            .ValidateAsync();
        if (validityCheckResult != null)
            return (accessToken, null, null);

        var newRefreshToken = await _refreshTokenService.RotateRefreshTokenAsync(
            oldEntity: tokenEntity!,
            ipAddress: ipAddress
        );

        return (accessToken, newRefreshToken, tokenEntity!.ExpiresAt);
    }
}