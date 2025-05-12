using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Interfaces.Auth;
using SmartRetail360.Application.Interfaces.Common;
using SmartRetail360.Infrastructure.Data;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Exceptions;
using SmartRetail360.Shared.Localization;
using SmartRetail360.Shared.Responses;

namespace SmartRetail360.Infrastructure.Services.Auth;

public class TenantAccountEmailVerificationService : IEmailVerificationService
{
    private readonly AppDbContext _dbContext;
    private readonly IUserContextService _userContext;
    private readonly MessageLocalizer _localizer;
    
    public TenantAccountEmailVerificationService(AppDbContext dbContext, IUserContextService userContext, MessageLocalizer localizer)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _localizer = localizer;
    }

    public async Task<ApiResponse<object>> VerifyEmailAsync(string token)
    {
        var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.EmailVerificationToken == token);
        
        if (tenant == null)
            throw new CommonException(ErrorCodes.InvalidTokenOrAccountAlreadyActivated);
        
        if (tenant.IsEmailVerified)
            throw new CommonException(ErrorCodes.AccountAlreadyActivated);
        
        tenant.IsEmailVerified = true;
        tenant.EmailVerificationToken = null;
        await _dbContext.SaveChangesAsync();
        
        return ApiResponse<object>.Ok(
            null,
            _localizer.GetSuccessMessage(SuccessCodes.AccountActivatedSuccessfully),
            traceId: _userContext.TraceId
        );
        
    }
}