namespace DbApp.Application.UserSystem.RideEntryRecords;

/// <summary>
/// DTO for ride entry record response.
/// </summary>
public class RideEntryRecordDto
{
    public int RideEntryRecordId { get; set; }
    public int VisitorId { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public int RideId { get; set; }
    public string RideName { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public string EntryGate { get; set; } = string.Empty;
    public string? ExitGate { get; set; }
    public int? TicketId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
