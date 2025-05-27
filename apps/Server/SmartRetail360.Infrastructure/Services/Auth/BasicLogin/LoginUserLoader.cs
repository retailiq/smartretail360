using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.UserLogin;

public class LoginUserLoader
{
    private readonly LoginContext _ctx;

    public LoginUserLoader(LoginContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<LoginResponse>?> LoadUserAsync()
    {
        var (user, userError) = await _ctx._dep.PlatformContext.GetUserByEmailAsync(_ctx.Request.Email);
        if (userError != null)
            return userError.To<LoginResponse>();

        var userCheckResult = await _ctx._dep.GuardChecker
            .Check(() => user == null, LogEventType.UserLoginFailure,
                LogReasons.AccountNotFound, ErrorCodes.AccountNotFound)
            .ValidateAsync();
        if (userCheckResult != null)
            return userCheckResult.To<LoginResponse>();

        _ctx.User = user;
        _ctx._dep.UserContext.Inject(new UserExecutionContext { UserId = user!.Id, UserName = user.Name });

        return null;
    }
}