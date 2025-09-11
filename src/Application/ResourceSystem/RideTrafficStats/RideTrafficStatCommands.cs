using MediatR;

namespace DbApp.Application.ResourceSystem.RideTrafficStats;

/// <summary>
/// Command to update traffic statistics for all rides.
/// </summary>
public record UpdateAllRideTrafficStatsCommand(
    DateTime RecordTime
) : IRequest<Unit>;

/// <summary>
/// Command to update traffic statistics for a specific ride.
/// </summary>
public record UpdateRideTrafficStatCommand(
    int RideId,
    DateTime RecordTime
) : IRequest<Unit>;
