using LoginTreasureApi.Database;
using LoginTreasureApi.Helpers;
using LoginTreasureApi.Interfaces;
using LoginTreasureApi.Models;
using LoginTreasureApi.Requests;
using LoginTreasureApi.Response;

namespace LoginTreasureApi.Services;

public class TokenService : ITokenService
{
    private readonly LoginDbContext loginDbContext;
    public TokenService(LoginDbContext loginDbContext)
    {
        this.loginDbContext = loginDbContext;
    }

    public async Task<Tuple<string, string>> GenerateTokenAsync(int userId)
    {
        var accessToken = await TokenHelper.GenerateAccessToken(userId);
        var refreshToken = await TokenHelper.GenerateRefreshToken();

        var userRecord = await loginDbContext.Users.Include(o => o.RefreshTokens)
            .FirstOrDefaultAsync(e => e.Id == userId);

        if (userRecord == null)
        {
            return null;
        }

        var salt = PasswordHelper.GetSecureSalt();
        var refreshTokenHash = PasswordHelper.HashingPassword(refreshToken, salt);

        if (userRecord.RefreshTokens != null & userRecord.RefreshTokens.Any())
        {
            await RemoveRefreshToken(userRecord);
        }
        userRecord.RefreshTokens?.Add(new RefreshToken
        {
            ExpiryDate = DateTime.Now.AddDays(30),
            Ts = DateTime.Now,
            UserId = userId,
            TokenHash = refreshTokenHash,
            TokenSalt = Convert.ToBase64String(salt)
        });

        await loginDbContext.SaveChangesAsync();

        var token = new Tuple<string, string>(accessToken, refreshToken);

        return token;
    }

    public async Task<bool> RemoveRefreshToken(User user)
    {
        var userRecord = await loginDbContext.Users.Include(o => o.RefreshTokens).FirstOrDefaultAsync(e => e.Id == user.Id);

        if (userRecord == null)
        {
            return false;
        }

        if (userRecord.RefreshTokens != null && userRecord.RefreshTokens.Any())
        {
            var currentRefreshToken = userRecord.RefreshTokens.First();

            loginDbContext.RefreshTokens.Remove(currentRefreshToken);
        }

        return false;
    }

    public async Task<ValidateTokenResponse> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var refreshToken = await loginDbContext.RefreshTokens
            .FirstOrDefaultAsync(o => o.UserId == refreshTokenRequest.UserId);

        var response = new ValidateTokenResponse();
        if (refreshToken == null)
        {
            response.Success = false;
            response.Error = "Invalid session or user is already logged out";
            response.ErrorCode = "R02";
            return response;
        }

        var refreshTokenToValidateHash = PasswordHelper.HashingPassword(refreshTokenRequest.RefreshToken, Convert.FromBase64String(refreshToken.TokenSalt));

        if (refreshToken.TokenHash != refreshTokenToValidateHash)
        {
            response.Success = false;
            response.Error = "Invalid refresh token";
            response.ErrorCode = "R03";
            return response;
        }

        if (refreshToken.ExpiryDate < DateTime.Now)
        {
            response.Success = false;
            response.Error = "Refresh token has expired";
            response.ErrorCode = "R04";
            return response;
        }

        response.Success = true;
        response.UserId = refreshToken.UserId;

        return response;
    }
}
