using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Logging.Abstractions;
using SmartRetail360.Platform.Interfaces;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.ABAC.Services.AbacPolicyService;

public class AbacPolicyGetAllService : IAbacPolicyGetAllService
{
    private readonly IPlatformContextService _platformContext;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;
    private readonly ILogDispatcher _logDispatcher;

    public AbacPolicyGetAllService(
        IPlatformContextService platformContext,
        IUserContextService userContext,
        MessageLocalizer localizer,
        ILogDispatcher logDispatcher)
    {
        _platformContext = platformContext;
        _userContext = userContext;
        _localizer = localizer;
        _logDispatcher = logDispatcher;
    }

    public async Task<ApiResponse<List<AbacPolicy>>> GetAllPoliciesForTenantAsync(Guid tenantId)
    {
        var (policies, error) = await _platformContext.GetAbacPoliciesByTenantIdAsync(tenantId);
        if (error != null)
            return error.To<List<AbacPolicy>>();

        await _logDispatcher.Dispatch(LogEventType.GetAbacPoliciesSuccess);

        return ApiResponse<List<AbacPolicy>>.Ok(
            policies,
            _localizer.GetLocalizedText(LocalizedTextKey.AbacPoliciesRetrieved),
            _userContext.TraceId
        );
    }
}