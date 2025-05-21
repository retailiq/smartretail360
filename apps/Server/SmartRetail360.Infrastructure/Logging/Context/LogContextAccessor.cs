using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Logging;

namespace SmartRetail360.Infrastructure.Logging.Context;

public class LogContextAccessor : ILogContextAccessor
{
    private readonly IUserContextService _userContext;

    public LogContextAccessor(IUserContextService userContext)
    {
        _userContext = userContext;
    }

    public string? TraceId => _userContext.TraceId;
    public string? ClientEmail => _userContext.ClientEmail;
    public string? Locale => _userContext.Locale;
    public Guid? UserId => _userContext.UserId;
    public Guid? TenantId => _userContext.TenantId;
    public Guid? RoleId => _userContext.RoleId;
    public string? Module => _userContext.Module;
    public string? IpAddress => _userContext.IpAddress;
    public string? AccountType => _userContext.AccountType?.ToString();
    public string? ErrorStack => _userContext.ErrorStack;
    public string? Action => _userContext.Action;
}