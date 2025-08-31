using DbApp.Domain.Entities.ResourceSystem;  
  
namespace DbApp.Domain.Interfaces.ResourceSystem;  
  
public interface IRideTrafficStatRepository  
{  
    // 只保留查询操作
    Task<RideTrafficStat?> GetByIdAsync(int rideId, DateTime recordTime);  
      
    // 搜索方法  
    Task<IEnumerable<RideTrafficStat>> SearchAsync(string? searchTerm, int page, int pageSize);  
    Task<int> CountAsync(string? searchTerm);  
    Task<IEnumerable<RideTrafficStat>> SearchByRideAsync(int rideId, int page, int pageSize);  
    Task<int> CountByRideAsync(int rideId);  
    Task<IEnumerable<RideTrafficStat>> SearchByCrowdedAsync(bool isCrowded, int page, int pageSize);  
    Task<int> CountByCrowdedAsync(bool isCrowded);  
    Task<object> GetStatsAsync(DateTime? startDate, DateTime? endDate);  
}