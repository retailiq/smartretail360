using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Login.Interfaces;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.Shared;

public class LoginResponseBuilder
{
    private readonly ILoginContextBase _ctx;

    public LoginResponseBuilder(ILoginContextBase ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<LoginResponse>> BuildSuccessResponseAsync()
    {
        var candidates = _ctx.TenantUsers!.Select(tu => new TenantLoginCandidate
        {
            TenantId = tu.TenantId.ToString(),
            TenantName = tu.Tenant!.Name ?? GeneralConstants.NotSet,
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

        await _ctx.Dep.SafeExecutor.ExecuteAsync(
            async () => { await _ctx.Dep.RedisOperation.ResetUserLoginFailuresAsync(_ctx.User.Email); },
            LogEventType.RedisError,
            LogReasons.RedisOperateFailed,
            ErrorCodes.DatabaseUnavailable
        );

        await _ctx.Dep.LogDispatcher.Dispatch(LogEventType.LoginSuccess);

        return ApiResponse<LoginResponse>.Ok(
            loginResponse,
            _ctx.Dep.Localizer.GetLocalizedText(LocalizedTextKey.UserLoginSuccess),
            _ctx.TraceId
        );
    }
}
