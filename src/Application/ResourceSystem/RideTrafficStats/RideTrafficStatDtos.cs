namespace DbApp.Application.ResourceSystem.RideTrafficStats;

/// <summary>
/// Summary DTO for ride traffic stat search results.
/// </summary>
public class RideTrafficStatSummaryDto
{
    public int RideId { get; set; }
    public string RideName { get; set; } = string.Empty;
    public DateTime RecordTime { get; set; }
    public int VisitorCount { get; set; }
    public int QueueLength { get; set; }
    public int WaitingTime { get; set; }
    public bool? IsCrowded { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Ride traffic statistics DTO.
/// </summary>
public class RideTrafficStatsDto
{
    public int TotalRecords { get; set; }
    public int CrowdedRecords { get; set; }
    public double AverageVisitorCount { get; set; }
    public double AverageQueueLength { get; set; }
    public double AverageWaitingTime { get; set; }
    public int MaxVisitorCount { get; set; }
    public int MaxQueueLength { get; set; }
    public int MaxWaitingTime { get; set; }
    public DateTime? FirstRecord { get; set; }
    public DateTime? LastRecord { get; set; }
}

/// <summary>
/// Search result containing ride traffic stats and pagination info.
/// </summary>
public class RideTrafficStatResult
{
    public List<RideTrafficStatSummaryDto> RideTrafficStats { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
