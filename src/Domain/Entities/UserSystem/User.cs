namespace DbApp.Domain.Entities.UserSystem;

/// <summary>
/// User entity representing the users table in the database.
/// </summary>
public class User
{
    /// <summary>
    /// User unique identifier.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Name of the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
