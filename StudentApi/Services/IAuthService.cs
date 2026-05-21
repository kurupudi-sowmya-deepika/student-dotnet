using StudentApi.Models.DTOs;

namespace StudentApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> Register(RegisterDto dto);
    Task<AuthResponseDto?> Login(LoginDto dto);
    Task<bool> ForgotPassword(ForgotPasswordDto dto);
    Task<List<UserDto>> GetAllUsers();
    Task<bool> UpdateUserRole(UpdateRoleDto dto);
}
