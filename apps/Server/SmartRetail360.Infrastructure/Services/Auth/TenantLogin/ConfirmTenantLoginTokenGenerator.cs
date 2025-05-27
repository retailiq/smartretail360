using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.TenantLogin;

public class ConfirmTenantLoginTokenGenerator
{
    private readonly ConfirmTenantLoginContext _ctx;

    public ConfirmTenantLoginTokenGenerator(ConfirmTenantLoginContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<ConfirmTenantLoginResponse>?> GenerateTokensAsync()
    {
        var user = _ctx.TenantUser!.User;

        _ctx.AccessToken = _ctx._dep.AccessTokenGenerator.GenerateToken(
            userId: _ctx.TenantUser.Id.ToString(),
            email: user!.Email,
            name: user.Name,
            tenantId: _ctx.TenantUser.TenantId.ToString(),
            roleId: _ctx.TenantUser.RoleId.ToString(),
            locale: user.Locale,
            traceId: _ctx.TraceId
        );

        var refreshTokenResult = await _ctx._dep.SafeExecutor.ExecuteAsync(
            () => _ctx._dep.RefreshTokenService.CreateRefreshTokenAsync(
                _ctx.TenantUser.UserId,
                _ctx.TenantUser.TenantId,
                _ctx._dep.UserContext.IpAddress,
                _ctx.RefreshTokenExpiryDays),
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
