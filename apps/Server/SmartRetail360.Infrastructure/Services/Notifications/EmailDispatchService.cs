using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Logging.Abstractions;
using SmartRetail360.Logging.Interfaces;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Notifications;

public class EmailDispatchService : IEmailDispatchService
{
    private readonly IAccountActivationEmailResendingService _resendingAccountActivationEmail;
    private readonly ILogDispatcher _dispatcher;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;

    public EmailDispatchService(
        IAccountActivationEmailResendingService resendingAccountActivationEmail,
        ILogDispatcher dispatcher,
        IUserContextService userContext,
        MessageLocalizer localizer
    )
    {
        _resendingAccountActivationEmail = resendingAccountActivationEmail;
        _dispatcher = dispatcher;
        _userContext = userContext;
        _localizer = localizer;
    }

    public async Task<ApiResponse<object>> DispatchAsync(EmailTemplate template, string email)
    {
        switch (template)
        {
            case EmailTemplate.UserRegistrationActivation:
                return await _resendingAccountActivationEmail.ResendEmailAsync(email);

            default:
                await _dispatcher.Dispatch(LogEventType.EmailSendFailure, LogReasons.EmailTemplateNotFound);
                return ApiResponse<object>.Fail(ErrorCodes.EmailTemplateNotFound,
                    _localizer.GetErrorMessage(ErrorCodes.EmailTemplateNotFound), _userContext.TraceId);
        }
    }
}