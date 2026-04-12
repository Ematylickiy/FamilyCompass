using FamilyCompass.Api.Auth.Contracts;
using FamilyCompass.Application.Auth.Commands;
using FamilyCompass.Application.Auth.Exceptions;
using FamilyCompass.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamilyCompass.Api.Auth;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.RegisterAsync(
                new RegisterCommand(request.Username, request.Password, request.ConfirmPassword),
                cancellationToken);
            return Results.Ok(new RegisterResponse(result.Id, result.UserName));
        }
        catch (UsernameAlreadyTakenException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.LoginAsync(
                new LoginCommand(request.Username, request.Password),
                cancellationToken);
            return Results.Ok(new LoginResponse(result.AccessToken));
        }
        catch (InvalidCredentialsException)
        {
            return Results.Unauthorized();
        }
    }
}
