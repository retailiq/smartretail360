using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Extensions;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Infrastructure.Services.Messaging;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Messaging.Payloads;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Common;

public class PlatformContextService : IPlatformContextService
{
    private readonly AppDbContext _db;
    private readonly ISafeExecutor _safeExecutor;
    private readonly IUserContextService _userContext;
    private readonly SqsEmailProducer _emailQueueProducer;

    public PlatformContextService(
        AppDbContext db,
        ISafeExecutor safeExecutor,
        IUserContextService context,
        SqsEmailProducer emailQueueProducer)
    {
        _db = db;
        _safeExecutor = safeExecutor;
        _userContext = context;
        _emailQueueProducer = emailQueueProducer;
    }

    public async Task<(User?, ApiResponse<object>?)> GetUserByIdAsync(Guid userId)
    {
        var result = await _safeExecutor.ExecuteAsync(
            () => _db.Users.FirstOrDefaultAsync(u => u.Id == userId),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        return (result.Response.Data, result.IsSuccess ? null : result.ToObjectResponse());
    }

    public async Task<(User?, ApiResponse<object>?)> GetUserByEmailAsync(string email)
    {
        var result = await _safeExecutor.ExecuteAsync(
            () => _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLowerInvariant()),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        return (result.Response.Data, result.IsSuccess ? null : result.ToObjectResponse());
    }

    public async Task<(List<TenantUser>?, ApiResponse<object>?)> GetTenantUserByTenantAndUserIdAsync(Guid userId,
        Guid tenantId)
    {
        var result = await _safeExecutor.ExecuteAsync(
            () => _db.TenantUsers.Where(tu => tu.UserId == userId && tu.TenantId == tenantId).ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        return (result.Response.Data, result.IsSuccess ? null : result.ToObjectResponse());
    }

    public async Task<(List<TenantUser>?, ApiResponse<object>?)> GetTenantUserByUserIdAsync(Guid userId)
    {
        var result = await _safeExecutor.ExecuteAsync(
            () => _db.TenantUsers.Where(tu => tu.UserId == userId).ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        return (result.Response.Data, result.IsSuccess ? null : result.ToObjectResponse());
    }
    
    public async Task<(List<AbacPolicy>?, ApiResponse<object>?)> GetAbacPoliciesByTenantIdAsync(Guid tenantId)
    {
        var result = await _safeExecutor.ExecuteAsync(
            () => _db.AbacPolicies.Where(p => p.TenantId == tenantId).ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        return (result.Response.Data, result.IsSuccess ? null : result.ToObjectResponse());
    }

    public async Task<(Tenant?, ApiResponse<object>?)> GetTenantAsync(Guid tenantId)
    {
        var result = await _safeExecutor.ExecuteAsync(
            () => _db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        return (result.Response.Data, result.IsSuccess ? null : result.ToObjectResponse());
    }
    
    public async Task<(RefreshToken?, ApiResponse<object>?)> GetRefreshTokenAsync(string token)
    {
        var result = await _safeExecutor.ExecuteAsync(
            () => _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        return (result.Response.Data, result.IsSuccess ? null : result.ToObjectResponse());
    }

    public async Task<(List<Tenant>?, ApiResponse<object>?)> GetTenantsByIdsAsync(List<Guid> tenantIds)
    {
        var result = await _safeExecutor.ExecuteAsync(
            () => _db.Tenants.Where(t => tenantIds.Contains(t.Id)).ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        return (result.Response.Data, result.IsSuccess ? null : result.ToObjectResponse());
    }

    public async Task<(OAuthAccount?, ApiResponse<object>?)> GetOAuthAccountAsync(string email,
        OAuthProvider provider)
    {
        var providerStr = provider.GetEnumMemberValue();
        var result = await _safeExecutor.ExecuteAsync(
            () => _db.OAuthAccounts.FirstOrDefaultAsync(t => t.Email == email && t.Provider == providerStr),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );
        return (result.Response.Data, result.IsSuccess ? null : result.ToObjectResponse());
    }

    public async Task<ApiResponse<object>?> SendRegistrationInvitationEmailAsync(string token,
        ActivationEmailPayload payload)
    {
        var result = await _safeExecutor.ExecuteAsync(
            async () =>
            {
                _userContext.ApplyTo(payload);
                await _emailQueueProducer.SendAsync(payload);
            },
            LogEventType.SqsError,
            LogReasons.SendSqsMessageFailed,
            ErrorCodes.EmailSendFailed
        );

        return result.IsSuccess ? null : result.ToObjectResponse();
    }
}