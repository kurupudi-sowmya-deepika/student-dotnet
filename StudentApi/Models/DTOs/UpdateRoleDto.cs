namespace StudentApi.Models.DTOs;

public class UpdateRoleDto
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}
