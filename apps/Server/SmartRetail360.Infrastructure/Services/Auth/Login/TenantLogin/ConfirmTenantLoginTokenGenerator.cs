using SmartRetail360.Application.Models;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.TenantLogin;

public class ConfirmTenantLoginTokenGenerator
{
    private readonly ConfirmTenantLoginContext _ctx;

    public ConfirmTenantLoginTokenGenerator(ConfirmTenantLoginContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<ConfirmTenantLoginResponse>?> GenerateTokensAsync()
    {
        var user = _ctx.TenantUser!.User!;

        _ctx.AccessToken = _ctx._dep.AccessTokenGenerator.GenerateToken(new AccessTokenCreationContext
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            UserName = user.Name,
            TenantId = _ctx.TenantUser.TenantId.ToString(),
            RoleId = _ctx.TenantUser.RoleId.ToString(),
            RoleName = _ctx.TenantUser.Role!.Name,
            TraceId = _ctx.TraceId,
            Environment = _ctx._dep.UserContext.Env.GetEnumMemberValue()
        });
        
        var refreshTokenResult = await _ctx._dep.SafeExecutor.ExecuteAsync(
            () => _ctx._dep.RefreshTokenService.CreateRefreshTokenAsync(new RefreshTokenCreationContext
            {
                UserId = _ctx.TenantUser.UserId,
                TenantId = _ctx.TenantUser.TenantId,
                IpAddress = _ctx._dep.UserContext.IpAddress,
                ExpiryDays = _ctx.RefreshTokenExpiryDays,
                Email = user.Email,
                UserName = user.Name,
                Locale = user.Locale,
                TraceId = _ctx.TraceId,
                RoleId = _ctx.TenantUser.RoleId,
                RoleName = _ctx.TenantUser.Role.Name,
                Env = _ctx._dep.UserContext.Env.GetEnumMemberValue()
            }),
            LogEventType.ConfirmTenantLoginFailure,
            LogReasons.RefreshTokenCreationFailed,
            ErrorCodes.InternalServerError
        );

        if (!refreshTokenResult.IsSuccess)
            return refreshTokenResult.ToObjectResponse().To<ConfirmTenantLoginResponse>();

        _ctx.RefreshToken = refreshTokenResult.Response.Data ?? Guid.NewGuid().ToString();
        return null;
    }
}