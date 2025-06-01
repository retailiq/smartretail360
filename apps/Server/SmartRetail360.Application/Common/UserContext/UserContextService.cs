using Microsoft.AspNetCore.Http;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Enums.AccessControl;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Application.Common.UserContext;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _http;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _http = httpContextAccessor;

        // Initialize properties from HttpContext
        if (_http.HttpContext != null)
        {
            UserId = TryParseGuid("UserId");
            TenantId = TryParseGuid("TenantId");
            RoleId = TryParseGuid("RoleId");
            TraceId = Get("TraceId");
            Locale = Get("Locale");
            Email = Get("Email");
            UserName = Get("UserName");
            IpAddress = ResolveIpAddress();
            RoleName = Get("RoleName");
            Env = Enum.TryParse<DefaultEnvironmentType>(Get("Env"), ignoreCase: true, out var parsedEnv)
                ? parsedEnv
                : DefaultEnvironmentType.Default;
        }

        if (string.IsNullOrWhiteSpace(TraceId))
        {
            TraceId = TraceIdGenerator.Generate(TraceIdPrefix.Get(TraceModule.General), GeneralConstants.Autogen);
        }

        LogId = Guid.NewGuid().ToString();
    }

    private string? Get(string key)
    {
        return _http.HttpContext?.Items[key]?.ToString();
    }

    private Guid? TryParseGuid(string key)
    {
        var value = Get(key);
        return Guid.TryParse(value, out var parsed) ? parsed : null;
    }

    private string ResolveIpAddress()
    {
        var context = _http.HttpContext;
        if (context == null)
            return GeneralConstants.Unknown;

        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            return forwarded.ToString().Split(',')[0].Trim();

        return context.Connection.RemoteIpAddress?.ToString() ?? GeneralConstants.Unknown;
    }

    // Change the properties to public setters to allow setting them from the constructor
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public string? TraceId { get; set; }
    public string? Locale { get; set; }
    public string? Module { get; set; }
    public string? Email { get; set; }
    public string IpAddress { get; set; } = GeneralConstants.Unknown;
    public string? ErrorStack { get; set; }
    public string? Action { get; set; }
    public string? RoleName { get; set; }
    public string? LogId { get; set; }
    public string? UserName { get; set; }
    public DefaultEnvironmentType Env { get; set; } = DefaultEnvironmentType.Default;
    public LogEventType? LogEventType { get; set; }
    public User? UserEntity { get; set; }
    public Tenant? TenantEntity { get; set; }
    public TenantUser? TenantUserEntity { get; set; }
    public Role? RoleEntity { get; set; }

    public void Inject(UserExecutionContext context)
    {
        if (context.UserId != null) UserId = context.UserId;
        if (context.TenantId != null) TenantId = context.TenantId;
        if (context.RoleId != null) RoleId = context.RoleId;
        if (!string.IsNullOrWhiteSpace(context.TraceId)) TraceId = context.TraceId;
        if (!string.IsNullOrWhiteSpace(context.Locale)) Locale = context.Locale;
        if (!string.IsNullOrWhiteSpace(context.Module)) Module = context.Module;
        if (!string.IsNullOrWhiteSpace(context.Email)) Email = context.Email;
        if (!string.IsNullOrWhiteSpace(context.ErrorStack)) ErrorStack = context.ErrorStack;
        if (!string.IsNullOrWhiteSpace(context.IpAddress)) IpAddress = context.IpAddress;
        if (!string.IsNullOrWhiteSpace(context.Action)) Action = context.Action;
        if (!string.IsNullOrWhiteSpace(context.RoleName)) RoleName = context.RoleName;
        if (!string.IsNullOrWhiteSpace(context.LogId)) LogId = context.LogId;
        if (!string.IsNullOrWhiteSpace(context.UserName)) UserName = context.UserName;
        if (context.LogEventType != null) LogEventType = context.LogEventType;
        if (context.UserEntity != null) UserEntity = context.UserEntity;
        if (context.TenantEntity != null) TenantEntity = context.TenantEntity;
        if (context.TenantUserEntity != null) TenantUserEntity = context.TenantUserEntity;
        if (context.RoleEntity != null) RoleEntity = context.RoleEntity;
    }

    public UserExecutionContext ToExecutionContext()
    {
        return new UserExecutionContext
        {
            UserId = UserId,
            TenantId = TenantId,
            RoleId = RoleId,
            TraceId = TraceId,
            Locale = Locale,
            Module = Module,
            Email = Email,
            ErrorStack = ErrorStack,
            IpAddress = IpAddress,
            Action = Action,
            RoleName = RoleName,
            LogId = LogId,
            UserName = UserName,
            LogEventType = LogEventType,
            UserEntity = UserEntity,
            TenantEntity = TenantEntity,
            TenantUserEntity = TenantUserEntity,
            RoleEntity = RoleEntity
        };
    }
}