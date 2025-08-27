using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IAmusementRideRepository
{
    Task<int> CreateAsync(AmusementRide ride);
    Task<AmusementRide?> GetByIdAsync(int rideId);
    Task<List<AmusementRide>> GetAllAsync();
    Task<List<AmusementRide>> GetByStatusAsync(RideStatus status);
    Task UpdateAsync(AmusementRide ride);
    Task DeleteAsync(AmusementRide ride);
}
