using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Shared.Responses;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;

namespace SmartRetail360.ABAC.Services.AbacPolicyService;

public class AbacPolicyService : IAbacPolicyService
{
    private readonly IAbacPolicyGetAllService _getAll;
    private readonly IAbacPolicyUpdateService _update;
    private readonly IAbacPolicySyncService _sync;
    private readonly IAbacPolicyTemplateCreateService _tplCreate;
    private readonly IAbacPolicyApplyTemplateService _tplApply;
    private readonly IAbacPolicyDefaultCreateService _defaultCreate;
    private readonly IAbacPolicyDeriveService _derive;

    public AbacPolicyService(
        IAbacPolicyGetAllService getAll,
        IAbacPolicyUpdateService update,
        IAbacPolicySyncService sync,
        IAbacPolicyTemplateCreateService tplCreate,
        IAbacPolicyApplyTemplateService tplApply,
        IAbacPolicyDefaultCreateService defaultCreate,
        IAbacPolicyDeriveService derive)
    {
        _getAll = getAll;
        _update = update;
        _sync = sync;
        _tplCreate = tplCreate;
        _tplApply = tplApply;
        _defaultCreate = defaultCreate;
        _derive = derive;
    }

    public Task<ApiResponse<List<AbacPolicy>>> GetAllPoliciesForTenantAsync(Guid tenantId)
        => _getAll.GetAllPoliciesForTenantAsync(tenantId);

    public Task<ApiResponse<AbacPolicy>> UpdatePolicyRuleJsonAsync(Guid policyId, UpdateAbacPolicyRuleJsonRequest request)
        => _update.UpdatePolicyRuleJsonAsync(policyId, request);

    public Task<ApiResponse<AbacPolicy>> UpdatePolicyStatusAsync(Guid policyId, UpdateAbacPolicyStatusRequest request)
        => _update.UpdatePolicyStatusAsync(policyId, request);
    
    public Task<int> SyncPolicyFromTemplateAsync(Guid templateId, string newRuleJson)
        => _sync.SyncPolicyFromTemplateAsync(templateId, newRuleJson);

    public Task CreatePoliciesFromTemplateAsync(Guid tenantId, string templateName)
        => _tplCreate.CreatePoliciesFromTemplateAsync(tenantId, templateName);

    public Task<int> ApplyPolicyTemplatesToTenantAsync(Guid tenantId)
        => _tplApply.ApplyPolicyTemplatesToTenantAsync(tenantId);

    public Task CreateDefaultPoliciesForTenantAsync(Guid tenantId, bool enableImmediately = false)
        => _defaultCreate.CreateDefaultPoliciesForTenantAsync(tenantId, enableImmediately);

    public Task<ApiResponse<object>> DerivePolicyFromTemplateAsync(DeriveAbacPolicyRequest request)
        => _derive.DerivePolicyFromTemplateAsync(request);
}