namespace DbApp.Domain.Statistics.UserSystem;

/// <summary>
/// Statistics for entry records, including flow and activity data.
/// </summary>
public class EntryRecordStats
{
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
}

/// <summary>
/// Grouped statistics for entry records, based on a specified grouping key (e.g., by day, by gate).
/// </summary>
public class GroupedEntryRecordStats
{
    public string GroupKey { get; set; } = string.Empty; // The value used for grouping.
    public string GroupName { get; set; } = string.Empty; // Display name for the group.
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
}
