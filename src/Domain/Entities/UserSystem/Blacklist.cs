namespace DbApp.Domain.Entities.UserSystem;

/// <summary>
/// Blacklist entity for recording visitors who are banned from the theme park.
/// </summary>
public class Blacklist
{
    /// <summary>
    /// Visitor ID who is blacklisted, also serves as the primary key.
    /// </summary>
    public int VisitorId { get; set; }

    /// <summary>
    /// Reason for adding the visitor to the blacklist.
    /// </summary>
    public string? BlacklistReason { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation property.
    public Visitor Visitor { get; set; } = null!;
}
