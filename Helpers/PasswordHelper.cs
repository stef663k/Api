using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace LoginTreasureApi.Models;

public class PasswordHelper
{
    public static byte[] GetSecureSalt()
    {
        return RandomNumberGenerator.GetBytes(32);
    }

    public static string HashingPassword(string password, byte[] salt)
    {
        byte[] key = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterationCount: 100000, 32);

        return Convert.ToBase64String(key);
    }
}
