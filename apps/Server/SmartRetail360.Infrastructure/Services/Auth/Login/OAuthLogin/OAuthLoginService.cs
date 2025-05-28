using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Infrastructure.Services.Auth.Login.Shared;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin;

public class OAuthLoginService : IOAuthLoginService
{
    private readonly OAuthLoginDependencies _dep;

    public OAuthLoginService(OAuthLoginDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<LoginResponse>> OAuthLoginAsync(OAuthLoginRequest request)
    {
        _dep.UserContext.Inject(new UserExecutionContext { Action = LogActions.OAuthLogin });
        var context = new OAuthLoginContext(_dep, request);
        var guardChecks = new LoginGuardCheckers(context);
        var profileGetter = new OAuthUserProfileGetter(context);
        var userResolver = new OAuthUserTenantResolver(context);
        var tenantProcessor = new LoginTenantProcessor(context);
        var responseBuilder = new LoginResponseBuilder(context);
        
        var result = await profileGetter.GetUserProfileAsync();
        if (result != null) return result;
        
        result = await userResolver.ResolveTenantUserAsync();
        if (result != null) return result;
        
        result = await guardChecks.CheckAccountStatusAsync();
        if (result != null) return result;
        
        result = await tenantProcessor.LoadTenantsAndRolesAsync();
        if (result != null) return result;
        
        var response = await responseBuilder.BuildSuccessResponseAsync();
        return response;
    }
}
