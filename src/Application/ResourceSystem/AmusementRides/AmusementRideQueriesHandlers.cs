using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.AmusementRides;

public class GetAmusementRideByIdQueryHandler(IAmusementRideRepository rideRepository)
    : IRequestHandler<GetAmusementRideByIdQuery, AmusementRide?>
{
    private readonly IAmusementRideRepository _rideRepository = rideRepository;

    public async Task<AmusementRide?> Handle(GetAmusementRideByIdQuery request, CancellationToken cancellationToken)
    {
        return await _rideRepository.GetByIdAsync(request.RideId);
    }
}

public class GetAllAmusementRidesQueryHandler(IAmusementRideRepository rideRepository)
    : IRequestHandler<GetAllAmusementRidesQuery, List<AmusementRide>>
{
    private readonly IAmusementRideRepository _rideRepository = rideRepository;

    public async Task<List<AmusementRide>> Handle(GetAllAmusementRidesQuery request, CancellationToken cancellationToken)
    {
        return await _rideRepository.GetAllAsync();
    }
}

public class GetAmusementRidesByStatusQueryHandler(IAmusementRideRepository rideRepository)
    : IRequestHandler<GetAmusementRidesByStatusQuery, List<AmusementRide>>
{
    private readonly IAmusementRideRepository _rideRepository = rideRepository;

    public async Task<List<AmusementRide>> Handle(GetAmusementRidesByStatusQuery request, CancellationToken cancellationToken)
    {
        return await _rideRepository.GetByStatusAsync(request.Status);
    }
}
