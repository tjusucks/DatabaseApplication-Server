using DbApp.Domain.Entities.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IMaintenanceRecordRepository
{
    Task<int> CreateAsync(MaintenanceRecord record);
    Task<MaintenanceRecord?> GetByIdAsync(int maintenanceId);
    Task<List<MaintenanceRecord>> GetAllAsync();
    Task<List<MaintenanceRecord>> GetByRideIdAsync(int rideId);
    Task<List<MaintenanceRecord>> GetPendingAsync();
    Task UpdateAsync(MaintenanceRecord record);
    Task DeleteAsync(MaintenanceRecord record);
}
