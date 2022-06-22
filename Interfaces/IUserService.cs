using LoginTreasureApi.Requests;
using LoginTreasureApi.Response;

namespace LoginTreasureApi.Interfaces;

public interface IUserService
{
    Task<TokenResponse> LoginAsync(LoginRequest loginRequest);
    Task<SignupResponse> SignupAsync(RegisterRequest signupRequest);
    Task<LogoutResponse> LogoutAsync(int userId);
}
