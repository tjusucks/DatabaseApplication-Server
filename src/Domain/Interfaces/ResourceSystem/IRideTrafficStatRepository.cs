using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IRideTrafficStatRepository
{
    // 基本CRUD操作
    Task<RideTrafficStat?> GetByIdAsync(int rideId, DateTime recordTime);
    Task<RideTrafficStat> AddAsync(RideTrafficStat stat);
    Task UpdateAsync(RideTrafficStat stat);
    Task DeleteAsync(RideTrafficStat stat);

    // 统一搜索方法
    Task<IEnumerable<RideTrafficStat>> SearchAsync(
        string? keyword,
        int? rideId,
        bool? isCrowded,
        int? minVisitorCount,
        int? maxVisitorCount,
        int? minQueueLength,
        int? maxQueueLength,
        int? minWaitingTime,
        int? maxWaitingTime,
        DateTime? recordTimeFrom,
        DateTime? recordTimeTo,
        int page,
        int pageSize);

    // 统一计数方法
    Task<int> CountAsync(
        string? keyword,
        int? rideId,
        bool? isCrowded,
        int? minVisitorCount,
        int? maxVisitorCount,
        int? minQueueLength,
        int? maxQueueLength,
        int? minWaitingTime,
        int? maxWaitingTime,
        DateTime? recordTimeFrom,
        DateTime? recordTimeTo);

    // 统计方法
    Task<RideTrafficStats> GetStatsAsync(DateTime? startDate, DateTime? endDate);
}
