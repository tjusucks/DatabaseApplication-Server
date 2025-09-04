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

    // 统一搜索方法  
    Task<IEnumerable<AmusementRide>> SearchAsync(
        string? searchTerm,
        RideStatus? status,
        string? location,
        int? managerId,
        int? minCapacity,
        int? maxCapacity,
        int? minHeightLimit,
        int? maxHeightLimit,
        DateTime? openDateFrom,
        DateTime? openDateTo,
        int page,
        int pageSize);

    // 统一计数方法  
    Task<int> CountAsync(
        string? searchTerm,
        RideStatus? status,
        string? location,
        int? managerId,
        int? minCapacity,
        int? maxCapacity,
        int? minHeightLimit,
        int? maxHeightLimit,
        DateTime? openDateFrom,
        DateTime? openDateTo);

    // 统计方法  
    Task<object> GetStatsAsync(DateTime? startDate, DateTime? endDate);
}
