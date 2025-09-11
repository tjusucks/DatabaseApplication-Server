using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Entities.ResourceSystem;

namespace DbApp.Domain.Entities.UserSystem;

/// <summary>
/// Entry record entity tracking visitor entry and exit information for a specific ride.
/// Used for ride flow statistics and security management.
/// </summary>
public class RideEntryRecord
{
    /// <summary>
    /// Ride entry record unique identifier.
    /// </summary>
    public int RideEntryRecordId { get; set; }

    /// <summary>
    /// Foreign key reference to the visitor.
    /// </summary>
    public int VisitorId { get; set; }

    /// <summary>
    /// Foreign key reference to the ride.
    /// </summary>
    public int RideId { get; set; }

    /// <summary>
    /// Timestamp when the visitor entered the ride.
    /// </summary>
    public DateTime EntryTime { get; set; }

    /// <summary>
    /// Timestamp when the visitor exited the ride.
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
    public AmusementRide Ride { get; set; } = null!;
}
