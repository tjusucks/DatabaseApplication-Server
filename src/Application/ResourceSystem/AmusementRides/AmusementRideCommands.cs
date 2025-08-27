using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.AmusementRides;

public record CreateAmusementRideCommand(
    string RideName,
    int? ManagerId,
    string Location,
    string? Description,
    RideStatus RideStatus,
    int Capacity,
    int Duration,
    int HeightLimitMin,
    int HeightLimitMax,
    DateTime? OpenDate
) : IRequest<int>;

public record UpdateAmusementRideCommand(
    int RideId,
    string RideName,
    int? ManagerId,
    string Location,
    string? Description,
    RideStatus RideStatus,
    int Capacity,
    int Duration,
    int HeightLimitMin,
    int HeightLimitMax,
    DateTime? OpenDate
) : IRequest<Unit>;

public record DeleteAmusementRideCommand(int RideId) : IRequest<Unit>;
