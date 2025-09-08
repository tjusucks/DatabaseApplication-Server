using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Application.ResourceSystem.InspectionRecords;

/// <summary>
/// Summary DTO for inspection record search results.
/// </summary>
public class InspectionRecordSummaryDto
{
    public int InspectionId { get; set; }
    public int RideId { get; set; }
    public string RideName { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public DateTime CheckDate { get; set; }
    public CheckType CheckType { get; set; }
    public bool IsPassed { get; set; }
    public string? IssuesFound { get; set; }
    public string? Recommendations { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }  // 添加更新时间
}

/// <summary>
/// Inspection record statistics DTO.
/// </summary>
public class InspectionRecordStatsDto
{
    public int TotalInspections { get; set; }
    public int PassedInspections { get; set; }
    public int FailedInspections { get; set; }
    public double PassRate { get; set; }
    public Dictionary<CheckType, int> InspectionTypeDistribution { get; set; } = new();
    public DateTime? FirstInspection { get; set; }
    public DateTime? LastInspection { get; set; }
}

/// <summary>
/// Search result containing inspection records and pagination info.
/// </summary>
public class InspectionRecordResult
{
    public List<InspectionRecordSummaryDto> InspectionRecords { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
