using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Contracts.AccountRegistration.Requests;
using SmartRetail360.Contracts.AccountRegistration.Responses;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.AccountRegistration.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Context;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Messaging.Factories;
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

    public async Task<ApiResponse<AccountRegisterResponse>> RegisterUserAsync(AccountRegisterRequest request)
    {
        _dep.UserContext.Inject(new UserExecutionContext
        {
            Email = request.Email,
            Action = LogActions.UserRegister
        });

        var slug = SlugGenerator.GenerateSlug(request.Email);

        var traceId = _dep.UserContext.TraceId;
        if (string.IsNullOrWhiteSpace(traceId))
        {
            traceId = TraceIdGenerator.Generate(TraceIdPrefix.Get(TraceModule.Auth), slug);
        }

        var lockKey = RedisKeys.RegisterAccountLock(request.Email.ToLower());
        var lockAcquired = await _dep.RedisOperation.AcquireLockAsync(lockKey,
            TimeSpan.FromSeconds(_dep.AppOptions.RegistrationLockTtlSeconds));

        var lockCheck = await _dep.GuardChecker
            .Check(() => !lockAcquired, LogEventType.RegisterUserFailure, LogReasons.LockNotAcquired,
                ErrorCodes.DuplicateRegisterAttempt)
            .ValidateAsync();

        if (lockCheck != null)
            return lockCheck.To<AccountRegisterResponse>();

        try
        {
            var (existingUser, userError) = await _dep.PlatformContext.GetUserByEmailAsync(request.Email);
            if (userError != null)
                return userError.To<AccountRegisterResponse>();

            if (existingUser != null)
            {
                _dep.UserContext.Inject(new UserExecutionContext
                {
                    UserId = existingUser.Id
                });
            }

            var guardResult = await _dep.GuardChecker
                .Check(() => existingUser is { IsEmailVerified: true }, LogEventType.RegisterUserFailure,
                    LogReasons.AccountAlreadyActivated, ErrorCodes.AccountAlreadyActivated)
                .Check(() => existingUser is { IsEmailVerified: false }, LogEventType.RegisterUserFailure,
                    LogReasons.AccountExistsButNotActivated, ErrorCodes.AccountExistsButNotActivated)
                .ValidateAsync();

            if (guardResult != null)
                return guardResult.To<AccountRegisterResponse>();

            var role = await _dep.RedisOperation.GetSystemRoleAsync(SystemRoleType.Admin);
            if (role == null)
                return ApiResponse<AccountRegisterResponse>.Fail(
                    ErrorCodes.DatabaseUnavailable,
                    _dep.Localizer.GetErrorMessage(ErrorCodes.DatabaseUnavailable), traceId);

            var roleId = role.Id;
            var roleName = role.Name;
            var passwordHash = PasswordHelper.HashPassword(request.Password);
            var emailVerificationToken = TokenHelper.GenerateActivateAccountToken();

            var user = new User
            {
                Email = request.Email.ToLowerInvariant(),
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
                TraceId = traceId,
                SourceEnum = ActivationSource.Registration
            };

            _dep.UserContext.Inject(new UserExecutionContext
            {
                TenantId = tenant.Id,
                UserId = user.Id,
                RoleId = tenantUser.RoleId,
                RoleName = RoleHelper.ToPascalCaseName(roleName)
            });

            var saveResult = await _dep.SafeExecutor.ExecuteAsync(
                async () =>
                {
                    _dep.Db.Users.Add(user);
                    _dep.Db.Tenants.Add(tenant);
                    _dep.Db.TenantUsers.Add(tenantUser);
                    _dep.Db.AccountActivationTokens.Add(accountActivationToken);
                    await _dep.Db.SaveChangesAsync();
                    await _dep.RedisOperation.SetActivationTokenAsync(accountActivationToken,
                        TimeSpan.FromMinutes(_dep.AppOptions.ActivationTokenLimitMinutes));
                },
                LogEventType.RegisterUserFailure,
                LogReasons.DatabaseSaveFailed,
                ErrorCodes.DatabaseUnavailable
            );

            if (!saveResult.IsSuccess)
                return saveResult.ToObjectResponse().To<AccountRegisterResponse>();

            var payload = ActivationEmailPayloadFactory.Create(
                user.Email,
                user.Name,
                emailVerificationToken,
                LogActions.UserRegistrationActivationEmailSend,
                EmailTemplate.UserRegistrationActivation,
                _dep.AppOptions.ActivationTokenLimitMinutes
            );
            
            var emailError =
                await _dep.PlatformContext.SendRegistrationInvitationEmailAsync(emailVerificationToken, payload);
            if (emailError != null)
                return emailError.To<AccountRegisterResponse>();

            await _dep.LogDispatcher.Dispatch(LogEventType.RegisterUserSuccess);

            return ApiResponse<AccountRegisterResponse>.Ok(
                new AccountRegisterResponse
                {
                    Status = user.Status,
                    Name = user.Name
                },
                _dep.Localizer.GetLocalizedText(LocalizedTextKey.AccountRegistered,
                    _dep.AppOptions.AccountActivationLimitMinutes),
                traceId
            );
        }
        finally
        {
            await _dep.RedisOperation.ReleaseLockAsync(lockKey);
        }
    }
}

// TODO: 初始化权限组/角色/默认设置