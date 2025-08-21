using DbApp.Domain.Entities.TicketingSystem;

namespace DbApp.Domain.Entities.UserSystem;

/// <summary>
/// Entry record entity tracking visitor entry and exit information.
/// Used for flow statistics and security management.
/// </summary>
public class EntryRecord
{
    /// <summary>
    /// Entry record unique identifier.
    /// </summary>
    public int EntryRecordId { get; set; }

    /// <summary>
    /// Foreign key reference to the visitor.
    /// </summary>
    public int VisitorId { get; set; }

    /// <summary>
    /// Timestamp when the visitor entered the park.
    /// </summary>
    public DateTime EntryTime { get; set; }

    /// <summary>
    /// Timestamp when the visitor exited the park.
    /// </summary>
    public DateTime? ExitTime { get; set; }

    /// <summary>
    /// Name of the entrance gate used.
    /// </summary>
    public string EntryGate { get; set; } = string.Empty;

    /// <summary>
    /// Name of the exit gate used.
    /// </summary>
    public string? ExitGate { get; set; }

    /// <summary>
    /// Foreign key reference to the ticket used for entry.
    /// </summary>
    public int? TicketId { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties.
    public Visitor Visitor { get; set; } = null!;
    public Ticket? Ticket { get; set; }
}
