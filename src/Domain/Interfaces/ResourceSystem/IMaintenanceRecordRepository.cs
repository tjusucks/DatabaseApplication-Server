using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IMaintenanceRecordRepository
{
    // 基本CRUD操作  
    Task<MaintenanceRecord?> GetByIdAsync(int maintenanceId);
    Task<MaintenanceRecord> AddAsync(MaintenanceRecord record);
    Task UpdateAsync(MaintenanceRecord record);
    Task DeleteAsync(MaintenanceRecord record);

    // 搜索方法  
    Task<IEnumerable<MaintenanceRecord>> SearchAsync(string? searchTerm, int page, int pageSize);
    Task<int> CountAsync(string? searchTerm);
    Task<IEnumerable<MaintenanceRecord>> SearchByRideAsync(int rideId, int page, int pageSize);
    Task<int> CountByRideAsync(int rideId);
    Task<IEnumerable<MaintenanceRecord>> SearchByStatusAsync(bool isCompleted, bool? isAccepted, int page, int pageSize);
    Task<int> CountByStatusAsync(bool isCompleted, bool? isAccepted);
    Task<object> GetStatsAsync(DateTime? startDate, DateTime? endDate);
}
