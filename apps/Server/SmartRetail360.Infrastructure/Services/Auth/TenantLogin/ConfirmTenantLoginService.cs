using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.TenantLogin;

public class ConfirmTenantLoginService : IConfirmTenantLoginService
{
    private readonly ConfirmTenantLoginDependencies _dep;

    public ConfirmTenantLoginService(ConfirmTenantLoginDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<ConfirmTenantLoginResponse>> ConfirmTenantLoginAsync(ConfirmTenantLoginRequest request)
    {
        var context = new ConfirmTenantLoginContext(_dep, request);
        var guards = new ConfirmTenantLoginGuardChecks(context);
        var tokens = new ConfirmTenantLoginTokenGenerator(context);
        var responseBuilder = new ConfirmTenantLoginResponseBuilder(context);

        var result = await guards.CheckTenantUserValidityAsync();
        if (result != null) return result;

        result = await tokens.GenerateTokensAsync();
        if (result != null) return result;

        result = await responseBuilder.FinalizeLoginAsync();
        return result!;
    }
}