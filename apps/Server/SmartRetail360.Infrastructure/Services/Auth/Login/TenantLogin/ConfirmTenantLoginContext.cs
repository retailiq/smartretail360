using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.Auth.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;

namespace SmartRetail360.Infrastructure.Services.Auth.Login.TenantLogin;

public class ConfirmTenantLoginContext
{
    public ConfirmTenantLoginDependencies _dep;
    public ConfirmTenantLoginRequest Request;
    public TenantUser? TenantUser;
    public string AccessToken = string.Empty;
    public string RefreshToken = string.Empty;
    public int RefreshTokenExpiryDays;
    public string TraceId => _dep.UserContext.TraceId;

    public ConfirmTenantLoginContext(ConfirmTenantLoginDependencies dep, ConfirmTenantLoginRequest request)
    {
        _dep = dep;
        Request = request;

        _dep.UserContext.Inject(new UserExecutionContext
        {
            Action = LogActions.ConfirmTenantLogin,
            UserId = request.UserId,
            TenantId = request.TenantId,
        });

        RefreshTokenExpiryDays = request.IsStaySignedIn == true
            ? _dep.AppOptions.RefreshTokenExpiryDaysWhenStaySignedIn
            : _dep.AppOptions.RefreshTokenExpiryDaysDefault;
    }
}