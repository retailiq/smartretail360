// using SmartRetail360.Application.Interfaces.Auth;
// using SmartRetail360.Application.Interfaces.Auth.Configuration;
// using SmartRetail360.Shared.Constants;
// using SmartRetail360.Shared.Enums;
// using SmartRetail360.Shared.Exceptions;
// using SmartRetail360.Shared.Responses;
//
// namespace SmartRetail360.Infrastructure.Services.Auth.Configuration;
//
// public class EmailVerificationDispatchService : IEmailVerificationDispatchService
// {
//     private readonly IEmailVerificationService _tenantAccountEmailVerificationService;
//
//     public EmailVerificationDispatchService(
//         IEmailVerificationService tenantAccountEmailVerificationService
//     )
//     {
//         _tenantAccountEmailVerificationService = tenantAccountEmailVerificationService;
//     }
//
//     public async Task<ApiResponse<object>> DispatchAsync(AccountType type, string token)
//     {
//         return type switch
//         {
//             // AccountType.TenantAccount => await _tenantAccountEmailVerificationService.VerifyEmailAsync(token),
//             _ => throw new CommonException(ErrorCodes.EmailTemplateNotFound)
//         };
//     }
// }