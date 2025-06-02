using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Common.Execution;
using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Auth.AccessControl;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Contracts.Auth.Requests;
using SmartRetail360.Domain.Entities.AccessControl;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.Auth.AccessControl;

public class AbacPolicyService : IAbacPolicyService
{
    private readonly AppDbContext _db;
    private readonly IPolicyRepo _policyRepo;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;
    private readonly IGuardChecker _guardChecker;
    private readonly ISafeExecutor _executor;
    private readonly IPlatformContextService _platformContext;

    public AbacPolicyService(
        AppDbContext db,
        IPolicyRepo policyRepo,
        IUserContextService userContext,
        MessageLocalizer localizer,
        IGuardChecker guardChecker,
        ISafeExecutor executor,
        IPlatformContextService platformContext)
    {
        _db = db;
        _policyRepo = policyRepo;
        _userContext = userContext;
        _localizer = localizer;
        _guardChecker = guardChecker;
        _executor = executor;
        _platformContext = platformContext;
    }

    public async Task<ApiResponse<List<AbacPolicy>>> GetAllPoliciesForTenantAsync(Guid tenantId)
    {
        var (policies, error) = await _platformContext.GetAbacPoliciesByTenantIdAsync(tenantId);
        if (error != null)
            return error.To<List<AbacPolicy>>();

        return ApiResponse<List<AbacPolicy>>.Ok(
            policies,
            _localizer.GetLocalizedText(LocalizedTextKey.AbacPoliciesRetrieved),
            _userContext.TraceId
        );
    }

    public async Task<ApiResponse<object>> UpdatePolicyAsync(UpdateAbacPolicyRequest request)
    {
        var invalidRuleResult = await _guardChecker
            .Check(() => !AbacPolicyHelp.IsValidRule(request.RuleJson), LogEventType.UpdateAbacPolicyFailure,
                LogReasons.InvalidAbacPolicyRule, ErrorCodes.InvalidAbacPolicyRule)
            .ValidateAsync();
        if (invalidRuleResult != null)
            return invalidRuleResult.To<object>();

        var existing = await _db.AbacPolicies.FindAsync(request.Id);

        var roleCheckResult = await _guardChecker
            .Check(() => existing == null, LogEventType.UpdateAbacPolicyFailure,
                LogReasons.AbacPolicyNotFound, ErrorCodes.AbacPolicyNotFound)
            .ValidateAsync();
        if (roleCheckResult != null)
            return roleCheckResult.To<object>();

        if (existing!.RuleJson == request.RuleJson)
        {
            return ApiResponse<object>.Ok(
                null,
                _localizer.GetLocalizedText(LocalizedTextKey.AbacPolicyUnchanged),
                _userContext.TraceId
            );
        }

        existing.RuleJson = request.RuleJson;
        existing.VersionNumber += 1;
        existing.UpdatedBy = _userContext.UserId ?? Guid.Empty;

        var saveResult = await _executor.ExecuteAsync(
            async () => { await _db.SaveChangesAsync(); },
            LogEventType.DatabaseError,
            LogReasons.DatabaseSaveFailed,
            ErrorCodes.DatabaseUnavailable
        );
        if (!saveResult.IsSuccess)
            return saveResult.ToObjectResponse().To<object>();

        return ApiResponse<object>.Ok(
            null,
            _localizer.GetLocalizedText(LocalizedTextKey.AbacPolicyUpdatedSuccessfully),
            _userContext.TraceId
        );
    }
    
    public async Task<int> SyncPolicyFromTemplateAsync(Guid templateId, string newRuleJson)
    {
        var now = DateTime.UtcNow;

        var targets = await _db.AbacPolicies
            .Where(p => p.TemplateId == templateId && p.AllowTemplateSync && p.RuleJson != newRuleJson)
            .ToListAsync();

        foreach (var policy in targets)
        {
            var backup = new AbacPolicy
            {
                TenantId = policy.TenantId,
                ResourceTypeId = policy.ResourceTypeId,
                ActionId = policy.ActionId,
                EnvironmentId = policy.EnvironmentId,
                AppliesToRole = policy.AppliesToRole,
                RuleJson = policy.RuleJson,
                IsEnabled = policy.IsEnabled,
                TemplateId = policy.TemplateId,
                BasePolicyId = policy.Id,
                VersionNumber = policy.VersionNumber,
                AllowTemplateSync = false, // 历史版本无需同步
                CreatedAt = policy.CreatedAt,
                UpdatedAt = policy.UpdatedAt,
                UpdatedBy = policy.UpdatedBy
            };
            _db.AbacPolicies.Add(backup);

            policy.RuleJson = newRuleJson;
            policy.VersionNumber += 1;
            policy.UpdatedAt = now;
        }

        return await _db.SaveChangesAsync();
    }

    public async Task CreatePoliciesFromTemplateAsync(Guid tenantId, string templateName)
    {
        var templates = await _db.AbacPolicyTemplates
            .Where(t => t.TemplateName == templateName && t.IsEnabled)
            .ToListAsync();

        if (!templates.Any()) return;

        var resourceMap = await _policyRepo.GetAllResourceTypeMapAsync();
        var actionMap = await _policyRepo.GetAllActionMapAsync();
        var envMap = await _policyRepo.GetAllEnvironmentMapAsync();

        var newPolicies = new List<AbacPolicy>();

        foreach (var tpl in templates)
        {
            if (!resourceMap.TryGetValue(tpl.ResourceType, out var resourceId) ||
                !actionMap.TryGetValue(tpl.Action, out var actionId) ||
                !envMap.TryGetValue(tpl.Environment, out var envId))
                continue;

            var exists = await _db.AbacPolicies.AnyAsync(p =>
                p.TenantId == tenantId &&
                p.ResourceTypeId == resourceId &&
                p.ActionId == actionId &&
                p.EnvironmentId == envId);

            if (exists) continue;

            newPolicies.Add(new AbacPolicy
            {
                TenantId = tenantId,
                ResourceTypeId = resourceId,
                ActionId = actionId,
                EnvironmentId = envId,
                RuleJson = tpl.RuleJson,
                IsEnabled = true,
                VersionNumber = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (newPolicies.Any())
        {
            await _db.AbacPolicies.AddRangeAsync(newPolicies);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<int> ApplyPolicyTemplatesToTenantAsync(Guid tenantId)
    {
        var templates = await _db.AbacPolicyTemplates.AsNoTracking()
            .Where(t => t.IsEnabled)
            .ToListAsync();

        var envMap = await _policyRepo.GetAllEnvironmentMapAsync();
        var resourceMap = await _policyRepo.GetAllResourceTypeMapAsync();
        var actionMap = await _policyRepo.GetAllActionMapAsync();

        var now = DateTime.UtcNow;

        var newPolicies = templates.Select(t =>
        {
            var resourceId = resourceMap.GetValueOrDefault(t.ResourceType);
            var actionId = actionMap.GetValueOrDefault(t.Action);
            var envId = envMap.GetValueOrDefault(t.Environment);

            if (resourceId == Guid.Empty || actionId == Guid.Empty || envId == Guid.Empty)
                return null;

            return new AbacPolicy
            {
                TenantId = tenantId,
                ResourceTypeId = resourceId,
                ActionId = actionId,
                EnvironmentId = envId,
                RuleJson = t.RuleJson,
                IsEnabled = t.IsEnabled,
                CreatedAt = now,
                UpdatedAt = now,
                UpdatedBy = Guid.Empty
            };
        }).Where(p => p != null).ToList()!;

        await _db.AbacPolicies.AddRangeAsync(newPolicies);
        await _db.SaveChangesAsync();

        return newPolicies.Count;
    }

    public async Task CreateDefaultPoliciesForTenantAsync(Guid tenantId)
    {
        var resourceMap = await _policyRepo.GetAllResourceTypeMapAsync();
        var actionMap = await _policyRepo.GetAllActionMapAsync();
        var envMap = await _policyRepo.GetAllEnvironmentMapAsync();
        var templates = await _policyRepo.GetPolicyTemplatesAsync();
        var resourceGroupMap = await _policyRepo.GetAllResourceGroupMapAsync();

        var environmentName = DefaultEnvironmentType.Client.GetEnumMemberValue();
        if (!envMap.TryGetValue(environmentName, out var environmentId))
        {
            Console.WriteLine($"[ABAC] 环境名 '{environmentName}' 无法映射到环境 ID");
            Console.WriteLine($"[ABAC] 当前环境映射表：{string.Join(", ", envMap.Keys)}");
            return;
        }

        foreach (var template in templates.Where(t => t.Environment == environmentName))
        {
            var expandedResources = new List<string>();
            
            if (resourceGroupMap.TryGetValue(template.ResourceType, out var groupResources))
            {
                expandedResources.AddRange(groupResources); // 将 data → dataset + report 展开
            }
            else if (resourceMap.ContainsKey(template.ResourceType))
            {
                expandedResources.Add(template.ResourceType); // 若不是 group，才当具体资源处理
            }

            Console.WriteLine($"[ABAC] 开始为租户 {tenantId} 创建策略...");

            foreach (var res in expandedResources)
            {
                Console.WriteLine($"[ABAC] 正在处理资源 {res}，动作 {template.Action}，环境 {environmentName}");

                if (!resourceMap.TryGetValue(res, out var resourceId))
                {
                    Console.WriteLine($"[ABAC] 未找到资源映射：{res}");
                    continue;
                }

                if (!actionMap.TryGetValue(template.Action.ToLowerInvariant(), out var actionId))
                {
                    Console.WriteLine($"[ABAC] 未找到动作映射：{template.Action}");
                    continue;
                }

                if (!envMap.TryGetValue(environmentName, out var envId))
                {
                    Console.WriteLine($"[ABAC] 未找到环境映射：{environmentName}");
                    continue;
                }

                bool exists = await _db.AbacPolicies.AnyAsync(p =>
                    p.TenantId == tenantId &&
                    p.ResourceTypeId == resourceId &&
                    p.ActionId == actionId &&
                    p.EnvironmentId == envId &&
                    p.VersionNumber == 1);

                if (exists)
                {
                    Console.WriteLine($"[ABAC] 策略已存在，跳过：资源 {res} 动作 {template.Action}");
                    continue;
                }

                Console.WriteLine($"[ABAC] 添加策略：资源 {res}, 动作 {template.Action}, 环境 {environmentName}");
                _db.AbacPolicies.Add(new AbacPolicy
                {
                    TenantId = tenantId,
                    ResourceTypeId = resourceId,
                    ActionId = actionId,
                    EnvironmentId = envId,
                    RuleJson = template.RuleJson,
                    TemplateId = template.Id,
                    IsEnabled = true,
                    AllowTemplateSync = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    VersionNumber = 1
                });
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<ApiResponse<object>> DerivePolicyFromTemplateAsync(DeriveAbacPolicyRequest request)
    {
        var template = (await _policyRepo.GetPolicyTemplatesAsync())
            .FirstOrDefault(t => t.TemplateName == request.TemplateName);

        if (template == null)
            return ApiResponse<object>.Fail(
                ErrorCodes.AbacPolicyNotFound,
                "指定的模板不存在或已被禁用",
                _userContext.TraceId
            );

        var resourceMap = await _policyRepo.GetAllResourceTypeMapAsync();
        var actionMap = await _policyRepo.GetAllActionMapAsync();
        var envMap = await _policyRepo.GetAllEnvironmentMapAsync();

        if (!resourceMap.TryGetValue(template.ResourceType, out var resourceId) ||
            !actionMap.TryGetValue(template.Action, out var actionId) ||
            !envMap.TryGetValue(template.Environment, out var envId))
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
            EnvironmentId = envId,
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