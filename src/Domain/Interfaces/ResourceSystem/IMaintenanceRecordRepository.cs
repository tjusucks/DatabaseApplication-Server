using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IMaintenanceRecordRepository
{
    // 基本CRUD操作
    Task<MaintenanceRecord?> GetByIdAsync(int maintenanceId);
    Task<MaintenanceRecord> AddAsync(MaintenanceRecord record);
    Task UpdateAsync(MaintenanceRecord record);
    Task DeleteAsync(MaintenanceRecord record);

    // 统一搜索方法
    Task<IEnumerable<MaintenanceRecord>> SearchAsync(
        string? keyword,
        int? rideId,
        int? teamId,
        int? managerId,
        MaintenanceType? maintenanceType,
        bool? isCompleted,
        bool? isAccepted,
        DateTime? startTimeFrom,
        DateTime? startTimeTo,
        DateTime? endTimeFrom,
        DateTime? endTimeTo,
        decimal? minCost,
        decimal? maxCost,
        int page,
        int pageSize);

    // 统一计数方法
    Task<int> CountAsync(
        string? keyword,
        int? rideId,
        int? teamId,
        int? managerId,
        MaintenanceType? maintenanceType,
        bool? isCompleted,
        bool? isAccepted,
        DateTime? startTimeFrom,
        DateTime? startTimeTo,
        DateTime? endTimeFrom,
        DateTime? endTimeTo,
        decimal? minCost,
        decimal? maxCost);

    // 统计方法
    Task<MaintenanceRecordStats> GetStatsAsync(DateTime? startDate, DateTime? endDate);
}
