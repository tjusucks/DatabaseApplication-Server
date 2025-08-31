using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Application.ResourceSystem.AmusementRides;

/// <summary>
/// Summary DTO for amusement ride search results.
/// </summary>
public class AmusementRideSummaryDto
{
    public int RideId { get; set; }
    public string RideName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RideStatus RideStatus { get; set; }
    public int Capacity { get; set; }
    public int Duration { get; set; }
    public int HeightLimitMin { get; set; }
    public int HeightLimitMax { get; set; }
    public DateTime? OpenDate { get; set; }
    public string? ManagerName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }  // 添加更新时间，与实体保持一致
}

/// <summary>
/// Amusement ride statistics DTO.
/// </summary>
public class AmusementRideStatsDto
{
    public int TotalRides { get; set; }
    public int OperationalRides { get; set; }
    public int MaintenanceRides { get; set; }
    public int ClosedRides { get; set; }
    public int TotalCapacity { get; set; }
    public double AverageCapacity { get; set; }
    public double AverageDuration { get; set; }
    public DateTime? FirstOpenDate { get; set; }
    public DateTime? LastOpenDate { get; set; }
}

/// <summary>
/// Search result containing amusement rides and pagination info.
/// </summary>
public class AmusementRideResult
{
    public List<AmusementRideSummaryDto> AmusementRides { get; set; } = new List<AmusementRideSummaryDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
