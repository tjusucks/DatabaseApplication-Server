using MediatR;

namespace DbApp.Application.ResourceSystem.RideTrafficStats;

/// <summary>
/// Query to get ride traffic stats by ride ID and record time.
/// </summary>
public record GetRideTrafficStatByIdQuery(int RideId, DateTime RecordTime) : IRequest<RideTrafficStatSummaryDto?>;

/// <summary>
/// Unified query to search ride traffic stats with comprehensive filtering options.
/// </summary>
public record SearchRideTrafficStatsQuery(
    string? Keyword = null,
    int? RideId = null,
    bool? IsCrowded = null,
    int? MinVisitorCount = null,
    int? MaxVisitorCount = null,
    int? MinQueueLength = null,
    int? MaxQueueLength = null,
    int? MinWaitingTime = null,
    int? MaxWaitingTime = null,
    DateTime? RecordTimeFrom = null,
    DateTime? RecordTimeTo = null,
    bool Descending = true,
    int Page = 1,
    int PageSize = 20
) : IRequest<RideTrafficStatResult>;

/// <summary>
/// Query to get ride traffic statistics.
/// </summary>
public record GetRideTrafficStatsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? RideId = null
) : IRequest<RideTrafficStatsDto>;

/// <summary>
/// Query to get real-time ride traffic statistics for a specific ride.
/// </summary>
public record GetRealTimeRideTrafficStatQuery(int RideId) : IRequest<RideTrafficStatSummaryDto>;

/// <summary>
/// Query to get real-time ride traffic statistics for all rides.
/// </summary>
public record GetAllRealTimeRideTrafficStatsQuery : IRequest<List<RideTrafficStatSummaryDto>>;
