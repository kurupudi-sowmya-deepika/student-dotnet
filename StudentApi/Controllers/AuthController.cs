using Microsoft.AspNetCore.Mvc;
using StudentApi.Models.DTOs;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("[controller]")]  // Routes: POST /auth/register  and  POST /auth/login
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST /auth/register
    // Creates a new user account. Returns a JWT token on success.
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var result = await _authService.Register(dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /auth/login
    // Validates credentials. Returns a JWT token on success.
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var result = await _authService.Login(dto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // POST /auth/forgot-password
    // Resets a user's password.
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var result = await _authService.ForgotPassword(dto);
        if (!result)
            return BadRequest(new { message = "User with this email does not exist." });

        return Ok(new { message = "Password updated successfully." });
    }
}
