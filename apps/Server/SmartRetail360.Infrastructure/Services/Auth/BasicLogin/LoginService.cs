using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.UserLogin;

public class LoginService : ILoginService
{
    private readonly LoginDependencies _dep;

    public LoginService(LoginDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        _dep.UserContext.Inject(new UserExecutionContext { Email = request.Email, Action = LogActions.UserLogin });

        var context = new LoginContext(_dep, request);

        var guardChecks = new LoginGuardChecks(context);
        var userLoader = new LoginUserLoader(context);
        var tenantProcessor = new LoginTenantProcessor(context);
        var responseBuilder = new LoginResponseBuilder(context);

        try
        {
            var result = await guardChecks.CheckRedisLocksAsync();
            if (result != null) return result;

            result = await userLoader.LoadUserAsync();
            if (result != null) return result;

            result = await guardChecks.CheckPasswordAndActivationAsync();
            if (result != null) return result;

            result = await tenantProcessor.LoadTenantsAndRolesAsync();
            if (result != null) return result;

            var response = await responseBuilder.BuildSuccessResponseAsync();
            return response;
        }
        finally
        {
            await _dep.RedisOperation.ReleaseLockAsync(context.LockKey);
        }
    }
}