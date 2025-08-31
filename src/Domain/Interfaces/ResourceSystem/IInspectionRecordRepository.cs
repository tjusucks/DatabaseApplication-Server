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
      
    // 搜索方法  
    Task<IEnumerable<InspectionRecord>> SearchAsync(string? searchTerm, int page, int pageSize);  
    Task<int> CountAsync(string? searchTerm);  
    Task<IEnumerable<InspectionRecord>> SearchByRideAsync(int rideId, int page, int pageSize);  
    Task<int> CountByRideAsync(int rideId);  
    Task<object> GetStatsAsync(DateTime? startDate, DateTime? endDate);  
}