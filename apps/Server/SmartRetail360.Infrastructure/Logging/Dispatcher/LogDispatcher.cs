using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Logging;

namespace SmartRetail360.Infrastructure.Logging.Dispatcher;

public class LogDispatcher : ILogDispatcher
{
    private readonly Dictionary<LogEventType, ILogEventHandler> _handlers;
    private readonly ILogActionResolver _actionResolver;
    private readonly IUserContextService _userContext;

    public LogDispatcher(IEnumerable<
        ILogEventHandler> handlers, 
        ILogActionResolver actionResolver,
        IUserContextService userContext)
    {
        _handlers = handlers.ToDictionary(h => h.EventType);
        _actionResolver = actionResolver;
        _userContext = userContext;
    }

    public Task Dispatch(LogEventType eventType, string? email = null, string? reason = null, string? errorStack = null)
    {
        if (_handlers.TryGetValue(eventType, out var handler))
        {
            var context = new LogContext
            {
                Email = email,
                Reason = reason,
                ErrorStack = errorStack,
                Action = _actionResolver.ResolveAction(eventType, _userContext.AccountType)
            };

            return handler.HandleAsync(context);
        }

        return Task.CompletedTask;
    }
}