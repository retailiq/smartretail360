using Microsoft.EntityFrameworkCore;
using SmartRetail360.Application.Extensions;
using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Contracts.AccountRegistration.Requests;
using SmartRetail360.Contracts.AccountRegistration.Responses;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.AccountRegistration.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Messaging.Payloads;
using SmartRetail360.Shared.Redis;
using SmartRetail360.Shared.Responses;
using SmartRetail360.Shared.Utils;

namespace SmartRetail360.Infrastructure.Services.AccountRegistration;

public class AccountRegistrationService : IAccountRegistrationService
{
    private readonly AccountRegistrationDependencies _dep;

    public AccountRegistrationService(AccountRegistrationDependencies dep)
    {
        _dep = dep;
    }

    public async Task<ApiResponse<AccountRegisterResponse>> RegisterAccountAsync(AccountRegisterRequest request)
    {
        _dep.UserContext.Inject(email: request.Email, action: LogActions.AccountRegister);

        var slug = SlugGenerator.GenerateSlug(request.Email);

        var traceId = _dep.UserContext.TraceId;
        if (string.IsNullOrWhiteSpace(traceId))
        {
            traceId = TraceIdGenerator.Generate(TraceIdPrefix.Get(TraceModule.Auth), slug);
        }

        var lockKey = RedisKeys.RegisterAccountLock(request.Email.ToLower());
        var lockAcquired = await _dep.RedisLockService.AcquireLockAsync(lockKey,
            TimeSpan.FromSeconds(_dep.AppOptions.RegistrationLockTtlSeconds));

        var lockCheck = await _dep.GuardChecker
            .Check(() => !lockAcquired, LogEventType.RegisterFailure, LogReasons.LockNotAcquired,
                ErrorCodes.DuplicateRegisterAttempt)
            .ValidateAsync();

        if (lockCheck != null)
            return lockCheck.To<AccountRegisterResponse>();

        try
        {
            var userResult = await _dep.SafeExecutor.ExecuteAsync(
                () => _dep.Db.Users.FirstOrDefaultAsync(t => t.Email == request.Email),
                LogEventType.RegisterFailure,
                LogReasons.DatabaseRetrievalFailed,
                ErrorCodes.DatabaseUnavailable
            );

            if (!userResult.IsSuccess)
                return userResult.ToObjectResponse().To<AccountRegisterResponse>();

            var existingUser = userResult.Response.Data;

            if (existingUser != null)
            {
                _dep.UserContext.Inject(userId: existingUser.Id);
            }

            var guardResult = await _dep.GuardChecker
                .Check(() => existingUser is { IsEmailVerified: true }, LogEventType.RegisterFailure,
                    LogReasons.AccountAlreadyActivated, ErrorCodes.AccountAlreadyActivated)
                .Check(() => existingUser is { IsEmailVerified: false }, LogEventType.RegisterFailure,
                    LogReasons.AccountExistsButNotActivated, ErrorCodes.AccountExistsButNotActivated)
                .ValidateAsync();

            if (guardResult != null)
                return guardResult.To<AccountRegisterResponse>();

            var role = await _dep.RoleCache.GetSystemRoleAsync(SystemRoleType.Admin);
            if (role == null)
                return ApiResponse<AccountRegisterResponse>.Fail(
                    ErrorCodes.DatabaseUnavailable,
                    _dep.Localizer.GetErrorMessage(ErrorCodes.DatabaseUnavailable), traceId);

            var roleId = role.Id;
            var roleName = role.Name;
            var passwordHash = PasswordHelper.HashPassword(request.Password);
            var emailVerificationToken = TokenGenerator.GenerateActivateAccountToken();

            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                PasswordHash = passwordHash,
                TraceId = traceId,
                LastEmailSentAt = DateTime.UtcNow
            };

            var tenant = new Tenant
            {
                TraceId = traceId,
                CreatedBy = user.Id,
            };

            var tenantUser = new TenantUser
            {
                UserId = user.Id,
                TenantId = tenant.Id,
                RoleId = roleId,
                TraceId = traceId,
                CreatedBy = user.Id,
            };

            var accountActivationToken = new AccountActivationToken
            {
                UserId = user.Id,
                Token = emailVerificationToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_dep.AppOptions.ActivationTokenLimitMinutes),
                TraceId = traceId
            };

            _dep.UserContext.Inject(
                tenantId: tenant.Id,
                userId: user.Id,
                roleId: tenantUser.RoleId,
                roleName: RoleHelper.ToPascalCaseName(roleName)
            );

            var saveResult = await _dep.SafeExecutor.ExecuteAsync(
                async () =>
                {
                    _dep.Db.Users.Add(user);
                    _dep.Db.Tenants.Add(tenant);
                    _dep.Db.TenantUsers.Add(tenantUser);
                    _dep.Db.AccountActivationTokens.Add(accountActivationToken);
                    await _dep.Db.SaveChangesAsync();
                    await _dep.ActivationTokenCache.SetTokenAsync(accountActivationToken,
                        TimeSpan.FromMinutes(_dep.AppOptions.ActivationTokenLimitMinutes));
                },
                LogEventType.RegisterFailure,
                LogReasons.DatabaseSaveFailed,
                ErrorCodes.DatabaseUnavailable
            );

            if (!saveResult.IsSuccess)
                return saveResult.ToObjectResponse().To<AccountRegisterResponse>();

            var emailResult = await _dep.SafeExecutor.ExecuteAsync(
                async () =>
                {
                    var payload = new ActivationEmailPayload
                    {
                        Email = user.Email,
                        Token = emailVerificationToken,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        Action = LogActions.AccountRegistrationActivateEmailSend,
                        EmailTemplate = EmailTemplate.AccountRegistrationActivation,
                        UserName = user.Name,
                    };

                    _dep.UserContext.ApplyTo(payload);

                    await _dep.EmailQueueProducer.SendAsync(payload);
                },
                LogEventType.RegisterFailure,
                LogReasons.SendSqsMessageFailed,
                ErrorCodes.EmailSendFailed
            );

            if (!emailResult.IsSuccess)
                return emailResult.ToObjectResponse().To<AccountRegisterResponse>();

            await _dep.LogDispatcher.Dispatch(LogEventType.RegisterSuccess);

            return ApiResponse<AccountRegisterResponse>.Ok(
                new AccountRegisterResponse
                {
                    Status = user.Status,
                    Name = user.Name
                },
                _dep.Localizer.GetLocalizedText(LocalizedTextKey.AccountRegistered),
                traceId
            );
        }
        finally
        {
            await _dep.RedisLockService.ReleaseLockAsync(lockKey);
        }
    }
}

// TODO: 初始化权限组/角色/默认设置