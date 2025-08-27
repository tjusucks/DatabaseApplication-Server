using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.AmusementRides;

public class CreateAmusementRideCommandHandler(IAmusementRideRepository rideRepository)
    : IRequestHandler<CreateAmusementRideCommand, int>
{
    private readonly IAmusementRideRepository _rideRepository = rideRepository;

    public async Task<int> Handle(CreateAmusementRideCommand request, CancellationToken cancellationToken)
    {
        var ride = new AmusementRide
        {
            RideName = request.RideName,
            ManagerId = request.ManagerId,
            Location = request.Location,
            Description = request.Description,
            RideStatus = request.RideStatus,
            Capacity = request.Capacity,
            Duration = request.Duration,
            HeightLimitMin = request.HeightLimitMin,
            HeightLimitMax = request.HeightLimitMax,
            OpenDate = request.OpenDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _rideRepository.CreateAsync(ride);
    }
}

public class UpdateAmusementRideCommandHandler(IAmusementRideRepository rideRepository)
    : IRequestHandler<UpdateAmusementRideCommand, Unit>
{
    private readonly IAmusementRideRepository _rideRepository = rideRepository;

    public async Task<Unit> Handle(UpdateAmusementRideCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetByIdAsync(request.RideId)
            ?? throw new InvalidOperationException("Amusement ride not found");

        ride.RideName = request.RideName;
        ride.ManagerId = request.ManagerId;
        ride.Location = request.Location;
        ride.Description = request.Description;
        ride.RideStatus = request.RideStatus;
        ride.Capacity = request.Capacity;
        ride.Duration = request.Duration;
        ride.HeightLimitMin = request.HeightLimitMin;
        ride.HeightLimitMax = request.HeightLimitMax;
        ride.OpenDate = request.OpenDate;
        ride.UpdatedAt = DateTime.UtcNow;

        await _rideRepository.UpdateAsync(ride);
        return Unit.Value;
    }
}

public class DeleteAmusementRideCommandHandler(IAmusementRideRepository rideRepository)
    : IRequestHandler<DeleteAmusementRideCommand, Unit>
{
    private readonly IAmusementRideRepository _rideRepository = rideRepository;

    public async Task<Unit> Handle(DeleteAmusementRideCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetByIdAsync(request.RideId)
            ?? throw new InvalidOperationException("Amusement ride not found");

        await _rideRepository.DeleteAsync(ride);
        return Unit.Value;
    }
}
