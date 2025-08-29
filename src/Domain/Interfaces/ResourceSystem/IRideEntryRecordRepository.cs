using DbApp.Domain.Entities.ResourceSystem;  
  
namespace DbApp.Domain.Interfaces.ResourceSystem;  
  
public interface IRideEntryRecordRepository  
{  
    Task<RideEntryRecord> CreateAsync(RideEntryRecord record);  
    Task<RideEntryRecord?> GetByIdAsync(int id);  
    Task<List<RideEntryRecord>> GetByRideIdAsync(int rideId);  
    Task<List<RideEntryRecord>> GetByVisitorIdAsync(int visitorId);  
    Task<List<RideEntryRecord>> GetFilteredAsync(int? rideId, int? visitorId, DateTime? startDate, DateTime? endDate, int page, int pageSize);  
    Task<List<RideEntryRecord>> GetCurrentVisitorsAsync();  
    Task<object> GetTrafficSummaryAsync(DateTime startDate, DateTime endDate, int? rideId);  
    Task UpdateAsync(RideEntryRecord record);  
    Task DeleteAsync(RideEntryRecord record);  
}