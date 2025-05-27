using System.Text.Json;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace SmartRetail360.Infrastructure.Services.Auth.OAuthLogin;

public class OAuthLoginService : IOAuthLoginService
{
    private readonly OAuthLoginDependencies _dep;

    public OAuthLoginService(OAuthLoginDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<LoginResponse>> OAuthLoginAsync(OAuthLoginRequest request)
    {
        _dep.UserContext.Inject(new UserExecutionContext { Action = LogActions.OAuthLogin });
        var context = new OAuthLoginContext(_dep, request);
        
       // 2. 获取 Handler
        var handler = _dep.OAuthProviderStrategy.Resolve(request.Provider);
        if (handler == null)
        {
            return ApiResponse<LoginResponse>.Fail(
                ErrorCodes.InvalidOAuthProvider,
                _dep.Localizer.GetErrorMessage(ErrorCodes.InvalidOAuthProvider),
                _dep.UserContext.TraceId
            );
        }

        // 3. 使用 handler 获取第三方用户信息
        var profileResult = await handler.GetUserProfileAsync(request);
        if (!profileResult.IsSuccess || profileResult.Profile == null)
        {
            return ApiResponse<LoginResponse>.Fail(
                ErrorCodes.OAuthUserProfileFetchFailed,
                profileResult.Error ?? _dep.Localizer.GetErrorMessage(ErrorCodes.OAuthUserProfileFetchFailed),
                _dep.UserContext.TraceId
            );
        }

        var profile = profileResult.Profile;
        
        var json = JsonSerializer.Serialize(
            profile,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
        Console.WriteLine("[OAuthLoginService] User profile fetched:\n{0}", json);

        // 4. 在数据库中查找现有用户（邮箱为主键）
        var (user, userError) = await _dep.PlatformContext.GetUserByEmailAsync(profile.Email);
        if (user == null || userError != null)
        {
            // TODO：支持自动注册逻辑（或提示“账号不存在”）
            return ApiResponse<LoginResponse>.Fail(
                ErrorCodes.AccountNotFound,
                _dep.Localizer.GetErrorMessage(ErrorCodes.AccountNotFound),
                _dep.UserContext.TraceId
            );
        }

        _dep.UserContext.Inject(new UserExecutionContext
        {
            Email = user.Email,
            UserId = user.Id,
            UserName = user.Name
        });

        // 5. 检查账号状态（Pending、Locked、Banned 等）
        var statusCheck = await _dep.GuardChecker
            .Check(() => user.StatusEnum == AccountStatus.PendingVerification, LogEventType.UserLoginFailure, LogReasons.AccountNotActivated, ErrorCodes.AccountNotActivated)
            .Check(() => user.StatusEnum == AccountStatus.Locked, LogEventType.UserLoginFailure, LogReasons.AccountLocked, ErrorCodes.AccountLocked)
            .Check(() => user.StatusEnum == AccountStatus.Banned, LogEventType.UserLoginFailure, LogReasons.AccountBanned, ErrorCodes.AccountBanned)
            .Check(() => user.StatusEnum == AccountStatus.Deleted, LogEventType.UserLoginFailure, LogReasons.AccountDeleted, ErrorCodes.AccountDeleted)
            .ValidateAsync();

        if (statusCheck != null)
            return statusCheck.To<LoginResponse>();

        // 6. 查找用户所属租户
        var tenantUsersResult = await _dep.SafeExecutor.ExecuteAsync(
            () => _dep.Db.TenantUsers
                .Where(tu => tu.UserId == user.Id)
                .Include(tu => tu.Tenant)
                .Include(tu => tu.Role)
                .ToListAsync(),
            LogEventType.DatabaseError,
            LogReasons.DatabaseRetrievalFailed,
            ErrorCodes.DatabaseUnavailable
        );

        if (!tenantUsersResult.IsSuccess)
            return tenantUsersResult.ToObjectResponse().To<LoginResponse>();

        var tenantUsers = tenantUsersResult.Response.Data!;
        if (tenantUsers.Count == 0)
        {
            return ApiResponse<LoginResponse>.Fail(
                ErrorCodes.TenantUserRecordNotFound,
                _dep.Localizer.GetErrorMessage(ErrorCodes.TenantUserRecordNotFound),
                _dep.UserContext.TraceId
            );
        }

        var candidates = tenantUsers.Select(tu => new TenantLoginCandidate
        {
            TenantId = tu.TenantId.ToString(),
            TenantName = tu.Tenant?.Name ?? GeneralConstants.Unknown,
            LogoUrl = tu.Tenant?.LogoUrl,
            RoleId = tu.RoleId.ToString(),
            RoleName = tu.Role?.Name ?? "Unknown",
            IsActive = tu.IsActive,
            IsDefault = tu.IsDefault
        }).ToList();

        return ApiResponse<LoginResponse>.Ok(
            new LoginResponse
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                ShouldChooseTenant = !candidates.Any(c => c.IsDefault),
                TenantOptions = candidates
            },
            _dep.Localizer.GetLocalizedText(LocalizedTextKey.UserLoginSuccess),
            _dep.UserContext.TraceId
        );
    }
}
