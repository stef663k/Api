using LoginTreasureApi.Database;
using LoginTreasureApi.Interfaces;
using LoginTreasureApi.Models;
using LoginTreasureApi.Requests;
using LoginTreasureApi.Response;

namespace LoginTreasureApi.Services;

public class UserService : IUserService
{
    private readonly LoginDbContext loginDbContext;
    private readonly ITokenService tokenService;
    public UserService(LoginDbContext loginDbContext, ITokenService tokenService)
    {
        this.loginDbContext = loginDbContext;
        this.tokenService = tokenService;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
    {
        var user = loginDbContext.Users.SingleOrDefault(user
            => user.Active && user.UserName == loginRequest.UserName);

        if(user == null)
        {
            return new TokenResponse
            {
                Success = false,
                Error = "Username not found",
                ErrorCode = "P04"
            };
        }

        var passwordHash = PasswordHelper.
            HashingPassword(loginRequest.Password, Convert.FromBase64String(user.PasswordSalt));
        if(user.Password != passwordHash)
        {
            return new TokenResponse
            {
                Success = false,
                Error = "Invalid password",
                ErrorCode = " P03"
            };
        }

        var token = await System.Threading.Tasks.Task.Run(() =>
            tokenService.GenerateTokenAsync(user.Id));

        return new TokenResponse
        {
            Success = true,
            AccessToken = token.Item1,
            RefreshToken = token.Item2
        };
    }

    public async Task<LogoutResponse> LogoutAsync(int userId)
    {
        var refreshToken = await loginDbContext.RefreshTokens
            .FirstOrDefaultAsync(o => o.UserId == userId);
        if(refreshToken == null)
        {
            return new LogoutResponse { Success = true };
        }
        loginDbContext.RefreshTokens.Remove(refreshToken);

        var saveResponse = await loginDbContext.SaveChangesAsync();

        if(saveResponse >= 0)
        {
            return new LogoutResponse { Success = true };
        }

        return new LogoutResponse { Success = false, Error = "Unable to logout user", ErrorCode = "L07" };
    }

    public async Task<SignupResponse> SignupAsync(RegisterRequest signupRequest)
    {
        var existingUser = await loginDbContext.Users
            .SingleOrDefaultAsync(user => user.UserName == signupRequest.UserName);
        if(existingUser != null)
        {
            new SignupResponse
            {
                Success = false,
                Error = " This username already exists",
                ErrorCode = "P06"
            };
        }
        if(signupRequest.Password != signupRequest.ConfirmPassword)
        {
            new SignupResponse
            {
                Success = false,
                Error = "Password and confirm password do not match",
                ErrorCode = "P08"
            };
        }
        if (signupRequest.Password.Length <= 8 )
        {
            new SignupResponse
            {
                Success = false,
                Error = "Password needs to be 8 chaeracters long",
                ErrorCode = " p010"
            };
        }

    }
}
