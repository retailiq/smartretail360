using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Shared.Utils;

public static class RoleHelper
{
    public static string GetRoleName(SystemRoleType roleEnum)
    {
        return StringCaseConverter.ToSnakeCase(roleEnum.ToString());
    }
    
    public static string ToPascalCaseName(string role)
    {
        var name = role.ToString().ToLowerInvariant();
        return char.ToUpper(name[0]) + name.Substring(1);
    }
}