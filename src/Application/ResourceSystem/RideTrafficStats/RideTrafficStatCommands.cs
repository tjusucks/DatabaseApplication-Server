using MediatR;

namespace DbApp.Application.ResourceSystem.RideTrafficStats;

public record CreateRideTrafficStatCommand(
    int RideId,
    DateTime RecordTime,
    int VisitorCount,
    int QueueLength,
    int WaitingTime,
    bool? IsCrowded
) : IRequest<Unit>;

public record UpdateRideTrafficStatCommand(
    int RideId,
    DateTime RecordTime,
    int VisitorCount,
    int QueueLength,
    int WaitingTime,
    bool? IsCrowded
) : IRequest<Unit>;

public record DeleteRideTrafficStatCommand(int RideId, DateTime RecordTime) : IRequest<Unit>;
