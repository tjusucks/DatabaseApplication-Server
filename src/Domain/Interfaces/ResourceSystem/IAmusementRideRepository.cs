using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IAmusementRideRepository
{
    // 基本CRUD操作  
    Task<AmusementRide?> GetByIdAsync(int rideId);
    Task<AmusementRide> AddAsync(AmusementRide ride);
    Task UpdateAsync(AmusementRide ride);
    Task DeleteAsync(AmusementRide ride);

    // 搜索方法  
    Task<IEnumerable<AmusementRide>> SearchAsync(string? searchTerm, int page, int pageSize);
    Task<int> CountAsync(string? searchTerm);
    Task<IEnumerable<AmusementRide>> SearchByStatusAsync(RideStatus status, int page, int pageSize);
    Task<int> CountByStatusAsync(RideStatus status);
    Task<object> GetStatsAsync(DateTime? startDate, DateTime? endDate);
}
