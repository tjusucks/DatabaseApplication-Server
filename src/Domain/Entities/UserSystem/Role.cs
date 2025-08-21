namespace DbApp.Domain.Entities.UserSystem;

/// <summary>
/// Role entity defining user permissions and access levels.
/// </summary>
public class Role
{
    /// <summary>
    /// Role unique identifier.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Unique name of the role (e.g., Admin, Manager, Employee, Visitor).
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the role's responsibilities and permissions.
    /// </summary>
    public string? RoleDescription { get; set; }

    /// <summary>
    /// Indicates whether this is a system-defined role or custom role.
    /// </summary>
    public bool IsSystemRole { get; set; } = false;

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation property.
    public ICollection<User> Users { get; set; } = [];
}
