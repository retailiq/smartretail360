using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Extensions;

namespace SmartRetail360.Logging.Services.Context;

public class LogContextAccessor : ILogContextAccessor
{
    private readonly IUserContextService _userContext;

    public LogContextAccessor(IUserContextService userContext)
    {
        _userContext = userContext;
    }

    public string TraceId => _userContext.TraceId;
    public string? Email => _userContext.Email;
    public string? Locale => _userContext.Locale;
    public Guid? UserId => _userContext.UserId;
    public Guid? TenantId => _userContext.TenantId;
    public Guid? RoleId => _userContext.RoleId;
    public string? Module => _userContext.Module;
    public string IpAddress => _userContext.IpAddress;
    public string? ErrorStack => _userContext.ErrorStack;
    public string? Action => _userContext.Action;
    public string? RoleName => _userContext.RoleName;
    public string? LogId => _userContext.LogId;
    public string? Env => _userContext.Env.GetEnumMemberValue();
}