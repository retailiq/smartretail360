using SmartRetail360.Application.Common.UserContext;
using SmartRetail360.Application.Interfaces.Logging;
using SmartRetail360.Application.Interfaces.Notifications;
using SmartRetail360.Application.Interfaces.Notifications.Configuration;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Notifications.Configuration;

public class EmailDispatchService : IEmailDispatchService
{
    private readonly IAccountActivateEmailResendingService _resendingTenantAccountActivateEmailService;
    private readonly ILogDispatcher _dispatcher;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;

    public EmailDispatchService(
        IAccountActivateEmailResendingService resendingTenantAccountActivateEmailService,
        ILogDispatcher dispatcher,
        IUserContextService userContext,
        MessageLocalizer localizer
    )
    {
        _resendingTenantAccountActivateEmailService = resendingTenantAccountActivateEmailService;
        _dispatcher = dispatcher;
        _userContext = userContext;
        _localizer = localizer;
    }

    public async Task<ApiResponse<object>> DispatchAsync(EmailTemplate template, string email)
    {
        _userContext.Inject(clientEmail: email);

        switch (template)
        {
            case EmailTemplate.TenantAccountActivation:
                return await _resendingTenantAccountActivateEmailService.ResendEmailAsync(email);

            default:
                await _dispatcher.Dispatch(LogEventType.EmailSendFailure, LogReasons.EmailTemplateNotFound);
                return ApiResponse<object>.Fail(ErrorCodes.EmailTemplateNotFound, _localizer.GetErrorMessage(ErrorCodes.EmailTemplateNotFound), _userContext.TraceId);
        }
    }
}