using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Login.Shared;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.CredentialsLogin;

public class CredentialsLoginService : ILoginService
{
    private readonly LoginDependencies _dep;

    public CredentialsLoginService(LoginDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        _dep.UserContext.Inject(new UserExecutionContext { Email = request.Email, Action = LogActions.UserLogin });

        var context = new CredentialsLoginContext(_dep, request);

        var guardChecks = new LoginGuardCheckers(context);
        var passwordChecker = new CredentialsLoginPasswordChecker(context);
        var userLoader = new CredentialsLoginUserLoader(context);
        var tenantProcessor = new LoginTenantProcessor(context);
        var responseBuilder = new LoginResponseBuilder(context);

        try
        {
            var result = await guardChecks.CheckRedisLocksAsync();
            if (result != null) return result;

            result = await userLoader.LoadUserAsync();
            if (result != null) return result;

            result = await passwordChecker.CheckPasswordAsync();
            if (result != null) return result;

            result = await guardChecks.CheckAccountStatusAsync();
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