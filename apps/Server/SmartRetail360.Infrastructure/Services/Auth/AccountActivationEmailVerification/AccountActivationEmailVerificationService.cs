using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Auth.AccountActivationEmailVerification;

public class AccountActivationEmailVerificationService : IAccountEmailVerificationService
{
    private readonly AccountActivationEmailVerificationDependencies _dep;

    public AccountActivationEmailVerificationService(AccountActivationEmailVerificationDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<object>> VerifyEmailAsync(string token)
    {
        var traceId = _dep.UserContext.TraceId;

        var (tokenEntity, user, tenant, tenantUser, policies, error) = await LoadEntitiesAsync(token);
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
        return await ActivateEntities(tokenEntity!, user!, tenant!, tenantUser!, token, traceId, policies);
    }

    private async Task<(AccountActivationToken?, User?, Tenant?, TenantUser?, List<AbacPolicy>?, ApiResponse<object>?)>
        LoadEntitiesAsync(string token)
    {
        var tokenCheckEntity = await _dep.RedisOperation.GetActivationTokenAsync(token);
        _dep.UserContext.Inject(new UserExecutionContext { UserId = tokenCheckEntity?.UserId });
        var checkResult = await _dep.GuardChecker
            .Check(() => tokenCheckEntity == null,
                LogEventType.AccountActivateFailure,
                LogReasons.InvalidToken,
                ErrorCodes.InvalidToken)
            .ValidateAsync();
        if (checkResult != null) return (null, null, null, null, null, checkResult);
        var action = tokenCheckEntity!.SourceEnum == ActivationSource.Registration
            ? LogActions.UserRegistrationActivate
            : LogActions.UserInvitationActivate;
        _dep.UserContext.Inject(new UserExecutionContext { Action = action });
        
        var (tenantUser, tenantUserError) =
            await _dep.PlatformContext.GetTenantUserRolesAsync(tenantId: tokenCheckEntity.TenantId,
                userId: tokenCheckEntity.UserId);

        if (tenantUserError != null) return (null, null, null, null, null, tenantUserError);
        
        if (tenantUser?.User == null || tenantUser.Tenant == null || tenantUser.Role == null)
        {
            return (null, null, null, null, null,
                ApiResponse<object>.Fail(ErrorCodes.TenantUserRecordNotFound,
                    _dep.Localizer.GetErrorMessage(ErrorCodes.TenantUserRecordNotFound), _dep.UserContext.TraceId));
        }

        var (policies, error) =
            await _dep.PlatformContext.GetAbacPoliciesByTenantIdAsync(tenantUser.TenantId);
        if (error != null)
            return (null, null, null, null, null, error);

        _dep.UserContext.Inject(new UserExecutionContext
        {
            UserId = tenantUser.User.Id,
            Email = tenantUser.User.Email,
            TenantId = tenantUser.Tenant.Id,
            RoleId = tenantUser.RoleId,
        });

        return (tokenCheckEntity, tenantUser.User, tenantUser.Tenant, tenantUser, policies, null);
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
        var isLimited = await _dep.RedisOperation.IsAccountActivationLimitedAsync(tokenStr);

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
        TenantUser tenantUser, string tokenStr, string traceId, List<AbacPolicy>? policies)
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

                foreach (var policy in policies ?? Enumerable.Empty<AbacPolicy>())
                {
                    policy.IsEnabled = true;
                }

                await _dep.Db.SaveChangesAsync();
                await _dep.RedisOperation.InvalidateActivationTokenAsync(tokenStr);
            },
            LogEventType.AccountActivateFailure,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );

        if (!result.IsSuccess) return result.ToObjectResponse();

        await _dep.RedisOperation.SetAccountActivationLimitAsync(tokenStr);
        await _dep.LogDispatcher.Dispatch(LogEventType.AccountActivateSuccess);

        return ApiResponse<object>.Ok(null,
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.AccountActivatedSuccessfully), traceId);
    }
}