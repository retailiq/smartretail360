using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.CredentialsLogin;

public class CredentialsLoginUserLoader
{
    private readonly CredentialsLoginContext _ctx;

    public CredentialsLoginUserLoader(CredentialsLoginContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<LoginResponse>?> LoadUserAsync()
    {
        var (user, userError) = await _ctx.Dep.PlatformContext.GetUserByEmailAsync(_ctx.Request.Email);
        if (userError != null)
            return userError.To<LoginResponse>();

        var userCheckResult = await _ctx.Dep.GuardChecker
            .Check(() => user == null, LogEventType.CredentialsLoginFailure,
                LogReasons.AccountNotFound, ErrorCodes.AccountNotFound)
            .ValidateAsync();
        if (userCheckResult != null)
            return userCheckResult.To<LoginResponse>();

        _ctx.User = user;
        _ctx.Dep.UserContext.Inject(new UserExecutionContext
            { UserId = user!.Id, UserName = user.Name, Email = user.Email });

        return null;
    }
}