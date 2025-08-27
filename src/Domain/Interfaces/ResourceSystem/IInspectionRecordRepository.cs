using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IInspectionRecordRepository
{
    Task<int> CreateAsync(InspectionRecord record);
    Task<InspectionRecord?> GetByIdAsync(int inspectionId);
    Task<List<InspectionRecord>> GetAllAsync();
    Task<List<InspectionRecord>> GetByRideIdAsync(int rideId);
    Task<List<InspectionRecord>> GetByTeamIdAsync(int teamId);
    Task<List<InspectionRecord>> GetFailedInspectionsAsync();
    Task<List<InspectionRecord>> GetByCheckTypeAsync(CheckType checkType);
    Task<List<InspectionRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task UpdateAsync(InspectionRecord record);
    Task DeleteAsync(InspectionRecord record);
}
