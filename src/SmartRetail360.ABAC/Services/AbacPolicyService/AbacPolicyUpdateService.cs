using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Execution;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Constants;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Logging.Abstractions;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Utils;
using SmartRetail360.Platform.Interfaces;

namespace SmartRetail360.ABAC.Services.AbacPolicyService;

public class AbacPolicyUpdateService : IAbacPolicyUpdateService
{
    private readonly AppDbContext _db;
    private readonly IGuardChecker _guardChecker;
    private readonly ISafeExecutor _executor;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;
    private readonly ILogDispatcher _logDispatcher;
    private readonly IPlatformContextService _platformContextService;

    public AbacPolicyUpdateService(
        AppDbContext db,
        IGuardChecker guardChecker,
        ISafeExecutor executor,
        IUserContextService userContext,
        MessageLocalizer localizer,
        ILogDispatcher logDispatcher,
        IPlatformContextService platformContextService)
    {
        _db = db;
        _guardChecker = guardChecker;
        _executor = executor;
        _userContext = userContext;
        _localizer = localizer;
        _logDispatcher = logDispatcher;
        _platformContextService = platformContextService;
    }

    public async Task<ApiResponse<AbacPolicy>> UpdatePolicyRuleJsonAsync(Guid policyId,
        UpdateAbacPolicyRuleJsonRequest request)
    {
        var invalidRuleResult = await _guardChecker
            .Check(() => !AbacPolicyHelp.IsValidRule(request.RuleJson), LogEventType.UpdateAbacPolicyFailure,
                LogReasons.InvalidAbacPolicyRule, ErrorCodes.InvalidAbacPolicyRule)
            .ValidateAsync();
        if (invalidRuleResult != null)
            return invalidRuleResult.To<AbacPolicy>();

        var (existingPolicy, error) = await _platformContextService.GetAbacPolicyByIdAsync(policyId);
        if (error != null)
            return error.To<AbacPolicy>();

        var policyCheckResult = await _guardChecker
            .Check(() => existingPolicy == null, LogEventType.UpdateAbacPolicyFailure,
                LogReasons.AbacPolicyNotFound, ErrorCodes.AbacPolicyNotFound)
            .Check(() => existingPolicy!.IsReplacedByNewVersion, LogEventType.UpdateAbacPolicyFailure,
                LogReasons.AbacPolicyHasBeenReplaced, ErrorCodes.AbacPolicyHasBeenReplaced)
            .Check(() => !existingPolicy!.IsEnabled, LogEventType.UpdateAbacPolicyFailure,
                LogReasons.AbacPolicyDisabled, ErrorCodes.AbacPolicyDisabled)
            .ValidateAsync();
        if (policyCheckResult != null)
            return policyCheckResult.To<AbacPolicy>();
        
        if (existingPolicy!.RuleJson == request.RuleJson)
        {
            await _logDispatcher.Dispatch(LogEventType.UpdateAbacPolicyFailure, LogReasons.AbacPolicyUnchanged);
            return ApiResponse<AbacPolicy>.Ok(
                null,
                _localizer.GetLocalizedText(LocalizedTextKey.AbacPolicyUnchanged),
                _userContext.TraceId
            );
        }

        existingPolicy.IsEnabled = false;
        existingPolicy.UpdatedBy = _userContext.UserId ?? Guid.Empty;
        existingPolicy.TemplateId = null;
        existingPolicy.AllowTemplateSync = false;
        existingPolicy.IsReplacedByNewVersion = true;

        var newAbacPolicy = new AbacPolicy
        {
            TenantId = existingPolicy.TenantId,
            ResourceTypeId = existingPolicy.ResourceTypeId,
            ActionId = existingPolicy.ActionId,
            RuleJson = request.RuleJson,
            IsEnabled = true,
            VersionNumber = existingPolicy.VersionNumber + 1,
            UpdatedBy = _userContext.UserId ?? Guid.Empty,
            BasePolicyId = existingPolicy.Id,
            AllowTemplateSync = false,
        };

        var saveResult = await _executor.ExecuteAsync(
            async () =>
            {
                await _db.AbacPolicies.AddAsync(newAbacPolicy);
                await _db.SaveChangesAsync();
            },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );
        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse().To<AbacPolicy>();

        await _logDispatcher.Dispatch(LogEventType.UpdateAbacPolicySuccess);

        return ApiResponse<AbacPolicy>.Ok(
            newAbacPolicy,
            _localizer.GetLocalizedText(LocalizedTextKey.AbacPolicyUpdatedSuccessfully),
            _userContext.TraceId
        );
    }

    public async Task<ApiResponse<AbacPolicy>> UpdatePolicyStatusAsync(Guid policyId,
        UpdateAbacPolicyStatusRequest request)
    {
        var (existingPolicy, error) = await _platformContextService.GetAbacPolicyByIdAsync(policyId);
        if (error != null)
            return error.To<AbacPolicy>();

        var policyCheckResult = await _guardChecker
            .Check(() => existingPolicy == null, LogEventType.UpdateAbacPolicyFailure,
                LogReasons.AbacPolicyNotFound, ErrorCodes.AbacPolicyNotFound)
            .Check(() => existingPolicy!.IsReplacedByNewVersion, LogEventType.UpdateAbacPolicyFailure,
                LogReasons.AbacPolicyHasBeenReplaced, ErrorCodes.AbacPolicyHasBeenReplaced)
            .ValidateAsync();
        if (policyCheckResult != null)
            return policyCheckResult.To<AbacPolicy>();

        if (existingPolicy!.IsEnabled == request.IsEnabled)
        {
            await _logDispatcher.Dispatch(LogEventType.UpdateAbacPolicyFailure, LogReasons.AbacPolicyUnchanged);
            return ApiResponse<AbacPolicy>.Ok(
                null,
                _localizer.GetLocalizedText(LocalizedTextKey.AbacPolicyUnchanged),
                _userContext.TraceId
            );
        }

        existingPolicy.IsEnabled = request.IsEnabled ?? !existingPolicy.IsEnabled;

        var saveResult = await _executor.ExecuteAsync(
            async () => { await _db.SaveChangesAsync(); },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );
        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse().To<AbacPolicy>();

        await _logDispatcher.Dispatch(LogEventType.UpdateAbacPolicySuccess);

        var returnData = existingPolicy.IsEnabled ? existingPolicy : null;
        var returnMessage = existingPolicy.IsEnabled
            ? _localizer.GetLocalizedText(LocalizedTextKey.AbacPolicyEnableSuccessfully)
            : _localizer.GetLocalizedText(LocalizedTextKey.AbacPolicyDisableSuccessfully);
        return ApiResponse<AbacPolicy>.Ok(
            returnData,
            returnMessage,
            _userContext.TraceId
        );
    }
}