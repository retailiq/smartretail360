using BCrypt.Net;

namespace SmartRetail360.Shared.Utils;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}