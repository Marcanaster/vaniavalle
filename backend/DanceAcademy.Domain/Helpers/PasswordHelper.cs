using System.Text;
using System.Linq;

namespace DanceAcademy.Domain.Helpers;

public static class PasswordHelper
{
    private static readonly char[] Lowercase = "abcdefghjkmnpqrstuvwxyz".ToCharArray();
    private static readonly char[] Uppercase = "ABCDEFGHJKMNPQRSTUVWXYZ".ToCharArray();
    private static readonly char[] Digits = "23456789".ToCharArray();
    private static readonly char[] Special = "!@#$%&*".ToCharArray();

    public static string GenerateRandomPassword(int length = 8)
    {
        if (length < 4) throw new ArgumentException("Length must be at least 4", nameof(length));

        var random = new Random();
        var password = new char[length];

        // Ensure at least one of each type
        password[0] = Lowercase[random.Next(Lowercase.Length)];
        password[1] = Uppercase[random.Next(Uppercase.Length)];
        password[2] = Digits[random.Next(Digits.Length)];
        password[3] = Special[random.Next(Special.Length)];

        var allChars = Lowercase.Concat(Uppercase).Concat(Digits).Concat(Special).ToArray();

        for (int i = 4; i < length; i++)
        {
            password[i] = allChars[random.Next(allChars.Length)];
        }

        // Shuffle
        return new string(password.OrderBy(_ => random.Next()).ToArray());
    }
}
