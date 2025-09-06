using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Application.UserSystem.Users;

/// <summary>
/// DTO for user query response.
/// </summary>
public class UserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
    public DateTime RegisterTime { get; set; }
    public int PermissionLevel { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
