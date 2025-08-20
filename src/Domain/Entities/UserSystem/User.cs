using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Domain.Entities.UserSystem;

/// <summary>
/// Core user entity representing the users table in the database.
/// Contains basic user information and authentication data.
/// </summary>
public class User
{
    /// <summary>
    /// User unique identifier.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Unique username for login purposes.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password for authentication.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// User's email address for communication and account recovery.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Display name shown to other users.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number for contact purposes.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's birth date for age verification and birthday promotions.
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// User's gender information.
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// Timestamp when the user account was registered.
    /// </summary>
    public DateTime RegisterTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Permission level controlling system access rights.
    /// </summary>
    [Range(0, 4)]
    public int PermissionLevel { get; set; } = 0;

    /// <summary>
    /// Foreign key reference to the user's role.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
