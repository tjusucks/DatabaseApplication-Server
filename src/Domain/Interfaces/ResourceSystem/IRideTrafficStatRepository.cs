using DbApp.Domain.Entities.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IRideTrafficStatRepository
{
    Task CreateAsync(RideTrafficStat stat);
    Task<RideTrafficStat?> GetByIdAsync(int rideId, DateTime recordTime);
    Task<List<RideTrafficStat>> GetByRideIdAsync(int rideId);
    Task<List<RideTrafficStat>> GetByDateRangeAsync(int rideId, DateTime startDate, DateTime endDate);
    Task<List<RideTrafficStat>> GetCrowdedRidesAsync(DateTime? date = null);
    Task<List<RideTrafficStat>> GetPopularityReportAsync(DateTime startDate, DateTime endDate);
    Task<List<RideTrafficStat>> GetPeakHoursAnalysisAsync(int rideId, DateTime date);
    Task UpdateAsync(RideTrafficStat stat);
    Task DeleteAsync(RideTrafficStat stat);
}
