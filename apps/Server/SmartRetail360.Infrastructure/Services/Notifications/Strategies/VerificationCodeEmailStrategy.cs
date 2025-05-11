// using SmartRetail360.Application.Interfaces.Notifications;
// using SmartRetail360.Application.Interfaces.Notifications.Strategies;
// using SmartRetail360.Domain.Entities;
// using SmartRetail360.Shared.Enums;
// using SmartRetail360.Shared.Options;
// using Microsoft.Extensions.Options;
//
// namespace SmartRetail360.Infrastructure.Services.Notifications.Strategies;
//
// public class VerificationCodeEmailStrategy : IEmailStrategy
// {
//     private readonly IEmailSender _emailSender;
//     private readonly AppOptions _appOptions;
//
//     public VerificationCodeEmailStrategy(IEmailSender emailSender, IOptions<AppOptions> options)
//     {
//         _emailSender = emailSender;
//         _appOptions = options.Value;
//     }
//
//     public async Task ExecuteAsync(Tenant tenant)
//     {
//         // 假设验证码是已经生成并写入到 Tenant 实体中的某个字段
//         if (string.IsNullOrWhiteSpace(tenant.EmailVerificationToken))
//         {
//             throw new InvalidOperationException("Verification code is missing");
//         }
//
//         var variables = new Dictionary<string, string>
//         {
//             ["code"] = tenant.EmailVerificationToken!
//         };
//
//         await _emailSender.SendAsync(
//             tenant.AdminEmail,
//             EmailTemplate.VerificationCode,
//             variables
//         );
//     }
// }