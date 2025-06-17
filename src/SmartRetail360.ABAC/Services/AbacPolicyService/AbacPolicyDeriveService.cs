using SmartRetail360.ABAC.Interfaces;
using SmartRetail360.ABAC.Interfaces.AbacPolicyService;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Persistence;
using SmartRetail360.Persistence.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.ABAC.Services.AbacPolicyService;

public class AbacPolicyDeriveService : IAbacPolicyDeriveService
{
    private readonly AppDbContext _db;
    private readonly IPolicyRepo _repo;
    private readonly IUserContextService _userContext;

    public AbacPolicyDeriveService(AppDbContext db, IPolicyRepo repo, IUserContextService userContext)
    {
        _db = db;
        _repo = repo;
        _userContext = userContext;
    }

    public async Task<ApiResponse<object>> DerivePolicyFromTemplateAsync(DeriveAbacPolicyRequest request)
    {
        var template = (await _repo.GetPolicyTemplatesMapAsync())
            .FirstOrDefault(t => t.TemplateName == request.TemplateName);

        if (template == null)
            return ApiResponse<object>.Fail(
                ErrorCodes.AbacPolicyNotFound,
                "指定的模板不存在或已被禁用",
                _userContext.TraceId
            );

        var resourceMap = await _repo.GetAllResourceTypeMapAsync();
        var actionMap = await _repo.GetAllActionMapAsync();

        if (!resourceMap.TryGetValue(template.ResourceType, out var resourceId) ||
            !actionMap.TryGetValue(template.Action, out var actionId))
            return ApiResponse<object>.Fail(
                ErrorCodes.AbacPolicyNotFound,
                "模板配置无效",
                _userContext.TraceId
            );

        _db.AbacPolicies.Add(new AbacPolicy
        {
            TenantId = request.TenantId,
            ResourceTypeId = resourceId,
            ActionId = actionId,
            RuleJson = template.RuleJson,
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return ApiResponse<object>.Ok(
            null,
            "模板派生策略创建成功",
            _userContext.TraceId
        );
    }
}
