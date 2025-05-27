using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Auth.UserLogin;

public class LoginResponseBuilder
{
    private readonly LoginContext _ctx;

    public LoginResponseBuilder(LoginContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<LoginResponse>> BuildSuccessResponseAsync()
    {
        var candidates = _ctx.TenantUsers!.Select(tu => new TenantLoginCandidate
        {
            TenantId = tu.TenantId.ToString(),
            TenantName = tu.Tenant!.Name ?? GeneralConstants.Unknown,
            LogoUrl = tu.Tenant.LogoUrl,
            RoleId = tu.RoleId.ToString(),
            RoleName = RoleHelper.ToPascalCaseName(tu.Role!.Name),
            IsDefault = tu.IsDefault,
            IsActive = tu.IsActive
        }).ToList();

        var shouldChooseTenant = !candidates.Any(c => c.IsDefault);

        var loginResponse = new LoginResponse
        {
            UserId = _ctx.User!.Id.ToString(),
            Email = _ctx.User.Email,
            ShouldChooseTenant = shouldChooseTenant,
            TenantOptions = candidates
        };

        await _ctx._dep.SafeExecutor.ExecuteAsync(
            async () => { await _ctx._dep.RedisOperation.ResetUserLoginFailuresAsync(_ctx.FailKey, _ctx.SecurityKey); },
            LogEventType.RedisError,
            LogReasons.RedisOperateFailed,
            ErrorCodes.DatabaseUnavailable
        );

        await _ctx._dep.LogDispatcher.Dispatch(LogEventType.UserLoginSuccess);

        return ApiResponse<LoginResponse>.Ok(
            loginResponse,
            _ctx._dep.Localizer.GetLocalizedText(LocalizedTextKey.UserLoginSuccess),
            _ctx.TraceId
        );
    }
}
