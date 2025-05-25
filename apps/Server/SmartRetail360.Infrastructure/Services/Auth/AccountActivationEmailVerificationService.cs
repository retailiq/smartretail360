using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Redis;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Auth;

public class AccountActivationEmailVerificationService : IAccountEmailVerificationService
{
    private readonly AuthDependencies _dep;

    public AccountActivationEmailVerificationService(AuthDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<object>> VerifyEmailAsync(string token)
    {
        var traceId = _dep.UserContext.TraceId;

        var (tokenEntity, user, tenant, tenantUser, error) = await LoadEntitiesAsync(token);
        if (error != null)
            return error;

        var idempotencyCheck = await CheckIdempotencyAsync(user);
        if (idempotencyCheck != null) return idempotencyCheck;

        if (await MarkTokenAsExpiredAsync(tokenEntity!, token))
            return ApiResponse<object>.Fail(ErrorCodes.TokenExpired,
                _dep.Localizer.GetErrorMessage(ErrorCodes.TokenExpired), traceId);

        var guardResult = await RunValidationGuards(tokenEntity!, token);
        if (guardResult != null) return guardResult;

        var role = await _dep.RedisOperation.GetSystemRoleAsync(SystemRoleType.Admin);
        _dep.UserContext.Inject(new UserExecutionContext
        {
            RoleName = RoleHelper.ToPascalCaseName(role?.Name ?? GeneralConstants.Unknown)
        });
        return await ActivateEntities(tokenEntity!, user!, tenant!, tenantUser!, token, traceId);
    }

    private async Task<(AccountActivationToken?, User?, Tenant?, TenantUser?, ApiResponse<object>?)>
        LoadEntitiesAsync(string token)
    {
        var tokenEntity = await _dep.RedisOperation.GetActivationTokenAsync(token);
        _dep.UserContext.Inject(new UserExecutionContext { UserId = tokenEntity?.UserId });
        var checkResult = await _dep.GuardChecker
            .Check(() => tokenEntity == null,
                LogEventType.AccountActivateFailure,
                LogReasons.InvalidToken,
                ErrorCodes.InvalidToken)
            .ValidateAsync();
        if (checkResult != null) return (null, null, null, null, checkResult);
        var action = tokenEntity!.SourceEnum == ActivationSource.Registration
            ? LogActions.UserRegistrationActivate
            : LogActions.UserInvitationActivate;
        _dep.UserContext.Inject(new UserExecutionContext { Action = action });

        var (user, userError) = await _dep.PlatformContext.GetUserByIdAsync(tokenEntity.UserId);
        if (userError != null) return (null, null, null, null,userError);

        var (tenantUser, tenantUserError) = await _dep.PlatformContext.GetTenantUserAsync(tokenEntity.UserId);
        if (tenantUserError != null) return (null, null, null, null,tenantUserError);

        Tenant? tenant = null;
        if (tenantUser != null)
        {
            var (tenantEntity, tenantError) = await _dep.PlatformContext.GetTenantAsync(tenantUser.TenantId);
            if (tenantError != null) return (null, null, null, null,tenantError);
            tenant = tenantEntity;
        }

        _dep.UserContext.Inject(new UserExecutionContext
        {
            UserId = user?.Id,
            Email = user?.Email,
            TenantId = tenant?.Id,
            RoleId = tenantUser?.RoleId
        });

        return (tokenEntity, user, tenant, tenantUser, null);
    }

    private async Task<ApiResponse<object>?> CheckIdempotencyAsync(User? user)
    {
        return await _dep.GuardChecker
            .Check(() => user is { IsEmailVerified: true, StatusEnum: AccountStatus.Active },
                LogEventType.AccountActivateFailure,
                LogReasons.AccountAlreadyActivated,
                ErrorCodes.AccountAlreadyActivated)
            .ValidateAsync();
    }

    private async Task<bool> MarkTokenAsExpiredAsync(AccountActivationToken tokenEntity, string token)
    {
        if (tokenEntity.ExpiresAt < DateTime.UtcNow && (tokenEntity.StatusEnum == ActivationTokenStatus.Pending))
        {
            tokenEntity.StatusEnum = ActivationTokenStatus.Expired;
            _dep.Db.Entry(tokenEntity).Property(e => e.Status).IsModified = true;

            var result = await _dep.SafeExecutor.ExecuteAsync(
                async () =>
                {
                    await _dep.Db.SaveChangesAsync();
                    await _dep.RedisOperation.InvalidateActivationTokenAsync(token);
                },
                LogEventType.AccountActivateFailure,
                LogReasons.DatabaseSaveFailed,
                ErrorCodes.DatabaseUnavailable
            );

            return !result.IsSuccess;
        }

        return false;
    }

    private async Task<ApiResponse<object>?> RunValidationGuards(AccountActivationToken token, string tokenStr)
    {
        var redisKey = RedisKeys.VerifyEmailRateLimit(tokenStr);
        var isLimited = await _dep.RedisOperation.IsLimitedAsync(redisKey);

        return await _dep.GuardChecker
            .Check(() => token.StatusEnum == ActivationTokenStatus.Used, LogEventType.AccountActivateFailure,
                LogReasons.TokenAlreadyUsed, ErrorCodes.TokenAlreadyUsed)
            .Check(() => token.StatusEnum == ActivationTokenStatus.Expired, LogEventType.AccountActivateFailure,
                LogReasons.TokenExpired, ErrorCodes.TokenExpired)
            .Check(() => token.StatusEnum == ActivationTokenStatus.Revoked, LogEventType.AccountActivateFailure,
                LogReasons.TokenRevoked, ErrorCodes.TokenRevoked)
            .Check(() => isLimited, LogEventType.AccountActivateFailure, LogReasons.TooFrequentActivationAttempt,
                ErrorCodes.TooFrequentActivationAttempt)
            .ValidateAsync();
    }

    private async Task<ApiResponse<object>> ActivateEntities(AccountActivationToken token, User user, Tenant tenant,
        TenantUser tenantUser, string tokenStr, string traceId)
    {
        user.IsEmailVerified = true;
        user.TraceId = traceId;
        user.StatusEnum = AccountStatus.Active;
        tenant.TraceId = traceId;
        tenant.StatusEnum = AccountStatus.Active;
        token.StatusEnum = ActivationTokenStatus.Used;
        token.TraceId = traceId;
        tenantUser.IsActive = true;

        var result = await _dep.SafeExecutor.ExecuteAsync(
            async () =>
            {
                _dep.Db.Entry(token).Property(e => e.Status).IsModified = true;
                await _dep.Db.SaveChangesAsync();
                await _dep.RedisOperation.InvalidateActivationTokenAsync(tokenStr);
            },
            LogEventType.AccountActivateFailure,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );

        if (!result.IsSuccess) return result.ToObjectResponse();

        var redisKey = RedisKeys.VerifyEmailRateLimit(tokenStr);
        await _dep.RedisOperation.SetLimitAsync(redisKey,
            TimeSpan.FromMinutes(_dep.AppOptions.AccountActivationLimitMinutes));
        await _dep.LogDispatcher.Dispatch(LogEventType.AccountActivateSuccess);

        return ApiResponse<object>.Ok(null,
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.AccountActivatedSuccessfully), traceId);
    }
}