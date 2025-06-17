using System.Net;
using SmartRetail360.Logging.Abstractions;
using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Notifications.Interfaces.Strategies;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Exceptions;

namespace SmartRetail360.Notifications.Services.Configuration;

public class EmailContext
{
    private readonly Dictionary<EmailTemplate, IEmailStrategy> _strategies;
    private readonly ILogDispatcher _dispatcher;

    public EmailContext(
        IEnumerable<IEmailStrategy> strategies,
        ILogDispatcher dispatcher
    )
    {
        _strategies = strategies.ToDictionary(s => s.StrategyKey);
        _dispatcher = dispatcher;
    }

    public Task SendAsync(EmailTemplate template, string toEmail, IDictionary<string, string> variables)
    {
        if (!_strategies.TryGetValue(template, out var strategy))
        {
            _dispatcher.Dispatch(LogEventType.EmailSendFailure, LogReasons.EmailStrategyNotFound);
            throw new CommonException(ErrorCodes.EmailStrategyNotFound, HttpStatusCode.ServiceUnavailable);
        }
        
        return strategy.ExecuteAsync(toEmail, variables);
    }
}