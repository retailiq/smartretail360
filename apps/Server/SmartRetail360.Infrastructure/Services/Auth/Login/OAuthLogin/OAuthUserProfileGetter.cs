using System.Text.Json;
using SmartRetail360.Contracts.Auth.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.OAuthLogin;

public class OAuthUserProfileGetter
{
    private readonly OAuthLoginContext _ctx;

    public OAuthUserProfileGetter(OAuthLoginContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResponse<LoginResponse>?> GetUserProfileAsync()
    {
        var request = _ctx.Request;

        // Get Handler
        var handler = _ctx.OAuthProviderStrategy.Resolve(request.Provider);

        var handlerCheckResult = await _ctx.Dep.GuardChecker
            .Check(() => handler == null,
                LogEventType.OAuthLoginFailure, LogReasons.OAuthHandlerNotFound,
                ErrorCodes.InternalServerError)
            .ValidateAsync();
        if (handlerCheckResult != null)
            return handlerCheckResult.To<LoginResponse>();

        // Use handler to fetch a user profile
        var profileResult = await _ctx.Dep.SafeExecutor.ExecuteAsync(
            () => handler!.GetUserProfileAsync(request),
            LogEventType.OAuthLoginFailure,
            LogReasons.OAuthUserProfileFetchFailed,
            ErrorCodes.OAuthUserProfileFetchFailed
        );

        if (!profileResult.IsSuccess)
            return profileResult.ToObjectResponse().To<LoginResponse>();

        var profile = profileResult.Response.Data;

        var profileCheckResult = await _ctx.Dep.GuardChecker
            .Check(() => profile == null,
                LogEventType.OAuthLoginFailure, LogReasons.OAuthUserProfileNotExists,
                ErrorCodes.OAuthUserProfileFetchFailed)
            .Check(() => profile!.Email == null,
                LogEventType.OAuthLoginFailure, LogReasons.TenantUserRecordNotFound,
                ErrorCodes.OAuthUserProfileNotExists)
            .ValidateAsync();
        if (profileCheckResult != null)
            return profileCheckResult.To<LoginResponse>();

        var json = JsonSerializer.Serialize(profile, JsonSerializerHelper.IndentedOptions);
        Console.WriteLine("[OAuthLoginService] User profile fetched:\n{0}", json);

        _ctx.UserProfile = profile!;

        return null;
    }
}