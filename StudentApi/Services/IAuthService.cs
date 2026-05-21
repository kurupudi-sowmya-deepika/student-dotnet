using StudentApi.Models.DTOs;

namespace StudentApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> Register(RegisterDto dto);
    Task<AuthResponseDto?> Login(LoginDto dto);
}
