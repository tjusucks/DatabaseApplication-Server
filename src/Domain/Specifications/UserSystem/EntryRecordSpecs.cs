namespace DbApp.Domain.Specifications.UserSystem;

/// <summary>
/// Specification for searching entry records with multiple criteria.
/// </summary>
public class EntryRecordSpec
{
    public string? Keyword { get; set; } // Search in entry, exit gate, visitor name etc.
    public int? VisitorId { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public DateTime? EntryTimeStart { get; set; }
    public DateTime? EntryTimeEnd { get; set; }
    public DateTime? ExitTimeStart { get; set; }
    public DateTime? ExitTimeEnd { get; set; }
    public string? EntryGate { get; set; }
    public string? ExitGate { get; set; }
    public int? TicketId { get; set; }
    public bool? IsActive { get; set; }
}
