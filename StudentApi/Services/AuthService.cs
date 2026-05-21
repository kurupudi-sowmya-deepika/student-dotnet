using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentApi.Data;
using StudentApi.Models;
using StudentApi.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    // AppDbContext and IConfiguration are injected by ASP.NET Core's DI container
    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponseDto?> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (!email.EndsWith("@intuceo.com"))
        {
            throw new ArgumentException("Email must belong to the @intuceo.com domain.");
        }

        // Prevent duplicate accounts — email must be unique
        if (await _context.Users.AnyAsync(u => u.Email == email))
            throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            Username = dto.Username,
            Email = email,
            // BCrypt hashes the password with a random salt — never store plain text
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User" // All new users get the "User" role by default
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return BuildResponse(user);
    }

    public async Task<AuthResponseDto?> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new KeyNotFoundException("User does not exist. Please sign up first.");
        }

        // BCrypt.Verify compares plain-text password against stored hash securely
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return BuildResponse(user);
    }

    public async Task<bool> ForgotPassword(ForgotPasswordDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    private AuthResponseDto BuildResponse(User user) => new()
    {
        Token = GenerateJwtToken(user),
        Username = user.Username,
        Email = user.Email,
        Role = user.Role
    };

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims are pieces of data embedded inside the JWT token.
        // The client receives these and the server can read them on every request.
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)   // Used by [Authorize(Roles = "Admin")]
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
            signingCredentials: credentials
        );

        // Serializes the token object into the compact "xxx.yyy.zzz" JWT string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
