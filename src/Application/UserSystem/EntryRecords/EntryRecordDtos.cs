using DbApp.Application.Common;

namespace DbApp.Application.UserSystem.EntryRecords;

/// <summary>
/// DTO for entry record response.
/// </summary>
public class EntryRecordDto
{
    public int EntryRecordId { get; set; }
    public int VisitorId { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public string EntryGate { get; set; } = string.Empty;
    public string? ExitGate { get; set; }
    public int? TicketId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for entry record statistics response.
/// </summary>
public class EntryRecordStatsDto
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
/// Result for search entry records query.
/// </summary>
public class SearchEntryRecordsResult : PaginatedResult<EntryRecordDto>
{
}

/// <summary>
/// DTO for grouped entry record statistics.
/// </summary>
public class GroupedEntryRecordStatsDto
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
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
