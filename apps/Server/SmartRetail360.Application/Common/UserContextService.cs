using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Application.Common;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<UserContextService> _logger;

    public UserContextService(IHttpContextAccessor httpContextAccessor, ILogger<UserContextService> logger)
    {
        _http = httpContextAccessor;
        _logger = logger;
    }
    
    private string? Get(string key)
    {
        return _http.HttpContext?.Items[key]?.ToString();
    }
    
    public Guid? UserId => TryParseGuid("UserId");
    public Guid? TenantId => TryParseGuid("TenantId");
    public Guid? RoleId => TryParseGuid("RoleId");
    public string? TraceId => Get("TraceId");
    public string? Locale => Get("Locale");
    public string? Module { get; set; }
    public string? ClientEmail => Get("ClientEmail");
    public AccountType? AccountType
    {
        get
        {
            var value = Get("AccountType");
            return Enum.TryParse<AccountType>(value, out var parsed) ? parsed : null;
        }
    }
    public string IpAddress
    {
        get
        {
            var context = _http.HttpContext;

            if (context == null)
                return "unknown";

            // 优先检查是否有代理转发头
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            {
                return forwarded.ToString().Split(',')[0].Trim(); // 拿第一个 IP
            }

            // 否则取直连 IP
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
    
    private Guid? TryParseGuid(string key)
    {
        var value = Get(key);
        return Guid.TryParse(value, out var parsed) ? parsed : null;
    }
    
    public void LogAllContext()
    {
        var context = _http.HttpContext;
        if (context?.Items != null)
        {
            foreach (var kvp in context.Items)
            {
                _logger.LogInformation("[UserContext] {Key} = {Value}", kvp.Key, kvp.Value);
            }
        }
    }
}