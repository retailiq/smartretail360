using SmartRetail360.Application.Interfaces.AccountRegistration;
using SmartRetail360.Contracts.AccountRegistration.Requests;
using SmartRetail360.Contracts.AccountRegistration.Responses;
using SmartRetail360.Domain.Entities;
using SmartRetail360.Infrastructure.Services.AccountRegistration.Models;
using SmartRetail360.Shared.Constants;
using SmartRetail360.Shared.Contexts.User;
using SmartRetail360.Shared.Enums;
using SmartRetail360.Shared.Extensions;
using SmartRetail360.Shared.Messaging.Factories;
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
        
        var lockAcquired = await _dep.RedisOperation.AcquireRegistrationLockAsync(request.Email.ToLower());
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
                _dep.UserContext.Inject(new UserExecutionContext { UserId = existingUser.Id });
            }

            var guardResult = await _dep.GuardChecker
                .Check(() => existingUser is { IsEmailVerified: true }, LogEventType.RegisterUserFailure,
                    LogReasons.AccountAlreadyActivated, ErrorCodes.AccountAlreadyActivated)
                .Check(() => existingUser is { IsEmailVerified: false }, LogEventType.RegisterUserFailure,
                    LogReasons.AccountExistsButNotActivated, ErrorCodes.AccountExistsButNotActivated)
                .ValidateAsync();
            if (guardResult != null)
                return guardResult.To<AccountRegisterResponse>();

            var role = await _dep.RedisOperation.GetSystemRoleAsync(SystemRoleType.Owner);
            var roleCheckResult = await _dep.GuardChecker
                .Check(() => role == null, LogEventType.RegisterUserFailure,
                    LogReasons.RoleListNotFound, ErrorCodes.InternalServerError)
                .ValidateAsync();
            if (roleCheckResult != null)
                return roleCheckResult.To<AccountRegisterResponse>();

            var roleId = role!.Id;
            var roleName = role.Name;
            var passwordHash = PasswordHelper.HashPassword(request.Password);
            var emailVerificationToken = TokenHelper.GenerateActivateAccountToken();

            var user = new User
            {
                Email = request.Email.ToLowerInvariant(),
                Name = request.Name,
                PasswordHash = passwordHash,
                TraceId = traceId,
                LastEmailSentAt = DateTime.UtcNow,
                Locale = string.IsNullOrWhiteSpace(_dep.UserContext.Locale)
                    ? LocaleType.En.GetEnumMemberValue()
                    : _dep.UserContext.Locale.ToEnumFromMemberValue<LocaleType>().GetEnumMemberValue()
            };

            var tenant = new Tenant
            {
                TraceId = traceId,
                CreatedBy = user.Id,
                Slug = slug,
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
                TenantId = tenant.Id,
                Token = emailVerificationToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_dep.AppOptions.AccountActivationLimitMinutes),
                TraceId = traceId,
                SourceEnum = ActivationSource.Registration
            };

            _dep.UserContext.Inject(new UserExecutionContext
            {
                TenantId = tenant.Id,
                UserId = user.Id,
                RoleId = tenantUser.RoleId,
                RoleName = RoleHelper.ToPascalCaseName(roleName),
                UserName = user.Name
            });

            var saveResult = await _dep.SafeExecutor.ExecuteAsync(
                async () =>
                {
                    _dep.Db.Users.Add(user);
                    _dep.Db.Tenants.Add(tenant);
                    _dep.Db.TenantUsers.Add(tenantUser);
                    _dep.Db.AccountActivationTokens.Add(accountActivationToken);
                    await _dep.Db.SaveChangesAsync();
                    await _dep.AbacPolicyService.CreateDefaultPoliciesForTenantAsync(tenant.Id);
                    await _dep.RedisOperation.SetActivationTokenAsync(accountActivationToken);
                },
                LogEventType.DatabaseError,
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
                _dep.AppOptions.AccountActivationLimitMinutes
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
            await _dep.RedisOperation.ReleaseRegistrationLockAsync(request.Email.ToLower());
        }
    }
}