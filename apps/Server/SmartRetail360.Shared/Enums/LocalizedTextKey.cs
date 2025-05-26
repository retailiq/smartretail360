namespace SmartRetail360.Shared.Enums;

public enum LocalizedTextKey
{
    AccountActivationTitle,
    AccountActivationGreeting,  
    AccountActivationInstruction,  
    AccountActivationCtaText,  
    AccountActivationFooter,
    AccountActivationSubject,
    PasswordChanged,
    AccountRegistered,
    AccountActivatedSuccessfully,
    EmailResent,
    AccountActivationValidityNotice,
    UserLoginSuccess,
    
    // Role descriptions
    RoleOwnerDescription,
    RoleAdminDescription,
    RoleAnalystDescription,
    RoleMemberDescription,
    RoleGuestDescription,
    
    // Field validation messages
    TokenIsRequired,
    InvalidTokenFormat,
    EmailIsRequired,
    InvalidEmailFormat,
    PasswordIsRequired,
    PasswordConfirmationIsRequired,
    PasswordMustBeAtLeast8Characters,
    PasswordMustContainUppercase,
    PasswordMustContainLowercase,
    PasswordMustContainDigit,
    PasswordMustContainSpecialCharacter,
    PasswordsDoNotMatch,
    InvalidEmailTemplate,
    NameIsRequired,
    NameMustBeAtLeast1Characters,
    NameMustNotExceed50Characters,
    TenantIdIsRequired,
    UserIdIsRequired,
}