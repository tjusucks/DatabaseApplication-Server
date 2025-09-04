using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IInspectionRecordRepository
{
    // 基本CRUD操作  
    Task<InspectionRecord?> GetByIdAsync(int inspectionId);
    Task<InspectionRecord> AddAsync(InspectionRecord record);
    Task UpdateAsync(InspectionRecord record);
    Task DeleteAsync(InspectionRecord record);

    // 统一搜索方法  
    Task<IEnumerable<InspectionRecord>> SearchAsync(
        string? searchTerm,
        int? rideId,
        int? teamId,
        CheckType? checkType,
        bool? isPassed,
        DateTime? checkDateFrom,
        DateTime? checkDateTo,
        int page,
        int pageSize);

    // 统一计数方法  
    Task<int> CountAsync(
        string? searchTerm,
        int? rideId,
        int? teamId,
        CheckType? checkType,
        bool? isPassed,
        DateTime? checkDateFrom,
        DateTime? checkDateTo);

    // 统计方法  
    Task<object> GetStatsAsync(DateTime? startDate, DateTime? endDate);
}
