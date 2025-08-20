namespace DbApp.Domain.Entities.UserSystem;

using DbApp.Domain.Enums.UserSystem;

/// <summary>
/// Visitor entity extending user information for theme park visitors.
/// Contains visitor-specific data like membership details and physical attributes.
/// </summary>
public class Visitor
{
    /// <summary>
    /// Visitor ID, which is also a foreign key to the User entity.
    /// </summary>
    public int VisitorId { get; set; }

    /// <summary>
    /// Type of visitor (regular or member).
    /// </summary>
    public VisitorType VisitorType { get; set; } = VisitorType.Regular;

    /// <summary>
    /// Accumulated membership points.
    /// </summary>
    public int Points { get; set; } = 0;

    /// <summary>
    /// Membership level classification.
    /// </summary>
    public string? MemberLevel { get; set; }

    /// <summary>
    /// Date when the visitor became a member.
    /// </summary>
    public DateTime? MemberSince { get; set; }

    /// <summary>
    /// Indicates whether the visitor is blacklisted.
    /// </summary>
    public bool IsBlacklisted { get; set; } = false;

    /// <summary>
    /// Visitor's height in centimeters for ride restrictions.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
