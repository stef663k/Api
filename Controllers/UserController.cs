﻿using LoginTreasureApi.Interfaces;
using LoginTreasureApi.Requests;
using LoginTreasureApi.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoginTreasureApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : BaseApiController
{
    private readonly IUserService userService;
    private readonly ITokenService tokenService;

    public UserController(IUserService userService, ITokenService tokenService)
    {
        this.userService = userService;
        this.tokenService = tokenService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UserName)
            || string.IsNullOrEmpty(loginRequest.Password))
        {
            return BadRequest(new TokenResponse
            {
                Error = "Missing login details",
                ErrorCode = "P12"
            });
        }

        var loginResponse = await userService.LoginAsync(loginRequest);

        if (!loginResponse.Success)
        {
            return Unauthorized(new
            {
                loginResponse.ErrorCode,
                loginResponse.Error
            });
        }

        return Ok(loginResponse);
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(x =>
                x.Errors.Select(c => c.ErrorMessage).ToList());
            if (errors.Any())
            {
                return BadRequest(new TokenResponse
                {
                    Error = $"{string.Join(",", errors)}",
                    ErrorCode = "P13"
                });
            }
        }

        var registerResponse = await userService.SignupAsync(registerRequest);
        if (!registerResponse.Success)
        {
            return UnprocessableEntity(registerResponse);
        }
        return Ok(registerResponse.UserName);
    }

    [HttpPost]
    [Route("refresh_token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        if (refreshTokenRequest == null || string.IsNullOrEmpty(refreshTokenRequest.RefreshToken) || refreshTokenRequest.UserId == 0)
        {
            return BadRequest(new TokenResponse
            {
                Error = "Missing refresh token details",
                ErrorCode = "R01"
            });
        }

        var validateRefreshTokenResponse = await tokenService.ValidateRefreshTokenAsync(refreshTokenRequest);

        if (!validateRefreshTokenResponse.Success)
        {
            return UnprocessableEntity(validateRefreshTokenResponse);
        }

        var tokenResponse = await tokenService.GenerateTokenAsync(validateRefreshTokenResponse.UserId);

        return Ok(new { AccessToken = tokenResponse.Item1, Refreshtoken = tokenResponse.Item2 });

    }

    [Authorize]
    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        var logout = await userService.LogoutAsync(UserID);

        if (!logout.Success)
        {
            return UnprocessableEntity(logout);
        }

        return Ok();
    }
}