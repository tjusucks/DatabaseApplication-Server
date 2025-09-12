namespace DbApp.Domain.Statistics.UserSystem;

/// <summary>
/// Statistics for ride entry records.
/// </summary>
public class RideEntryRecordStats
{
    public int? RideEntryRecordId { get; set; }
    public string? RideName { get; set; } = string.Empty;
    public int TotalEntries { get; set; }
    public int TotalExits { get; set; }
    public int ActiveEntries { get; set; }
    public int UniqueVisitors { get; set; }
    public DateTime? FirstEntryTime { get; set; }
    public DateTime? LastEntryTime { get; set; }
    public DateTime? FirstExitTime { get; set; }
    public DateTime? LastExitTime { get; set; }
    public int EntryGateCount { get; set; }
    public int ExitGateCount { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
