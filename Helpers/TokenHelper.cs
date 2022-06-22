using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LoginTreasureApi.Helpers;

public class TokenHelper
{
    public const string Issuer = "https://localhost:7287";
    public const string Audience = "https://localhost:7287";
    public const string Key = "p0GXO6VuVZLRPef0tyO9jCqK4uZufDa6LP4n8Gj+8hQPB30f94pFiECAnPeMi5N6VT3/uscoGH7+zJrv4AuuPg==";

    public static async Task<string> GenerateAccessToken(int userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Convert.FromBase64String(Key);

        var claims = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        });

        var creds = new SigningCredentials(new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var toeknDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Issuer = Issuer,
            Audience = Audience,
            Expires = DateTime.Now.AddMinutes(30),
            SigningCredentials = creds
        };
        var securityToken = tokenHandler.CreateToken(toeknDescriptor);

        return await System.Threading.Tasks.Task.Run(() 
            => tokenHandler.WriteToken(securityToken));
    }

    public static async Task<string> GenerateRefreshToken()
    {
        var secureRandomBytes = new byte[32];

        using var randomNumberGenrator = RandomNumberGenerator.Create();
        await System.Threading.Tasks.Task.Run(() =>
        randomNumberGenrator.GetBytes(secureRandomBytes));

        var refreshToken = Convert.ToBase64String(secureRandomBytes);

        return refreshToken;
    }
}
