using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;

using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        UserDto request)
    {
        var isValid =
            await _authService.ValidateUserAsync(
                request.Login,
                request.Password);

        if (!isValid)
        {
            return Unauthorized(new
            {
                message =
                    "Неверный логин или пароль"
            });
        }

        return Ok(new
        {
            message = "Успешный вход"
        });
    }
}