using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.AmusementRides;

public record GetAmusementRideByIdQuery(int RideId) : IRequest<AmusementRide?>;

public record GetAllAmusementRidesQuery() : IRequest<List<AmusementRide>>;

public record GetAmusementRidesByStatusQuery(RideStatus Status) : IRequest<List<AmusementRide>>;
