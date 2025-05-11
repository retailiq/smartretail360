using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SmartRetail360.Application.Interfaces.Common;

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