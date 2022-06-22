using LoginTreasureApi.Models;
using LoginTreasureApi.Requests;
using LoginTreasureApi.Response;

namespace LoginTreasureApi.Interfaces;

public interface ITokenService
{
    Task<Tuple<string, string>> GenerateTokenAsync(int userId);
    Task<ValidateTokenResponse> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    Task<bool> RemoveRefreshToken(User user);
}
