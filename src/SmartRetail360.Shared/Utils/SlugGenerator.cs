using System.Security.Cryptography;

namespace SmartRetail360.Shared.Utils;


public static class SlugGenerator
{
    public static string GenerateSlug(string email)
    {
        var baseSlug = email.Split('@')[0].ToLower().Replace(".", "-").Replace("_", "-");
        var suffix = GenerateShortId(); // like "a3f2"
        return $"{baseSlug}-{suffix}";
    }

    private static string GenerateShortId()
    {
        // Generate a random 2-byte value and convert it to a hexadecimal string
        Span<byte> buffer = stackalloc byte[2];
        RandomNumberGenerator.Fill(buffer);
        return BitConverter.ToString(buffer.ToArray()).Replace("-", "").ToLower();
    }
}