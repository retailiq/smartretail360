using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth;

public class ConfirmTenantLoginService : IConfirmTenantLoginService
{
    private readonly ConfirmTenantLoginDependencies _dep;

    public ConfirmTenantLoginService(ConfirmTenantLoginDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<ConfirmTenantLoginResponse>> ConfirmTenantLoginAsync(
        ConfirmTenantLoginRequest request)
    {
        var tenantUser = await _dep.Db.TenantUsers
            .Include(tu => tu.Tenant)
            .Include(tu => tu.Role)
            .Include(tu => tu.User)
            .FirstOrDefaultAsync(tu =>
                tu.UserId == request.UserId &&
                tu.TenantId == request.TenantId &&
                tu.IsActive);

        var userCheckResult = await _dep.GuardChecker
            .Check(() => tenantUser == null, LogEventType.UserLoginFailure,
                LogReasons.AccountNotFound, ErrorCodes.AccountNotFound)
            .ValidateAsync();
        if (userCheckResult != null)
            return userCheckResult.To<ConfirmTenantLoginResponse>();

        var isFirstLogin = tenantUser!.User?.IsFirstLogin ?? false;
        
        var accessToken = _dep.JwtTokenGenerator.GenerateToken(
            userId: tenantUser.Id.ToString(),
            email: tenantUser.Email,
            name: tenantUser.Name,
            tenantId: tenantUser!.TenantId.ToString(),
            roleId: tenantUser.RoleId.ToString(),
            locale: user.Locale.GetEnumMemberValue(),
            traceId: _dep.UserContext.TraceId
        );

        var refreshToken = TokenHelper.GenerateRefreshToken();

        user.LastLoginAt = DateTime.UtcNow;
        user.TraceId = traceId;
        user.IsFirstLogin = false;

        // return ApiResponse<ConfirmTenantLoginResponse>.Ok(
        //     new ConfirmTenantLoginResponse
        //     {
        //         // AccessToken = accessToken,
        //         RefreshToken = refreshToken,
        //         ExpiresIn = _dep.AppOptions.JwtExpirySeconds,
        //         User = new AuthUserInfo
        //         {
        //             UserId = user.Id.ToString(),
        //             Email = user.Email,
        //             Name = user.Name,
        //             Locale = user.Locale.GetEnumMemberValue(),
        //             // TenantId = tenantUser.TenantId.ToString(),
        //             // RoleId = tenantUser.RoleId.ToString(),
        //             AvatarUrl = user.AvatarUrl ?? string.Empty,
        //             IsFirstLogin = isFirstLogin,
        //             Permissions = new List<string>()
        //             // Permissions = await _dep.PlatformContext.GetUserPermissionsAsync(user.Id, tenantUser.RoleId)
        //         }
        //     },
        //     _dep.Localizer.GetLocalizedText(LocalizedTextKey.UserLoginSuccess),
        //     traceId
        // );
    }
}