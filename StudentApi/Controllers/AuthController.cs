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
        var result = await _authService.Register(dto);
        if (result == null)
            return BadRequest(new { message = "Email already exists." });

        return Ok(result);
    }

    // POST /auth/login
    // Validates credentials. Returns a JWT token on success.
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.Login(dto);
        if (result == null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(result);
    }
}
