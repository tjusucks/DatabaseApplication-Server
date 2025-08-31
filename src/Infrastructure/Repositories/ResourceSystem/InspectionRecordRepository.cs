using DbApp.Domain.Entities.ResourceSystem;  
using DbApp.Domain.Interfaces.ResourceSystem;  
using Microsoft.EntityFrameworkCore;  
  
namespace DbApp.Infrastructure.Repositories.ResourceSystem;  
  
public class InspectionRecordRepository(ApplicationDbContext dbContext) : IInspectionRecordRepository  
{  
    private readonly ApplicationDbContext _dbContext = dbContext;  
  
    public async Task<InspectionRecord?> GetByIdAsync(int inspectionId)  
    {  
        return await _dbContext.InspectionRecords  
            .Include(r => r.Ride)  
            .Include(r => r.Team)  
            .FirstOrDefaultAsync(r => r.InspectionId == inspectionId);  
    }  
  
    public async Task<InspectionRecord> AddAsync(InspectionRecord record)  
    {  
        _dbContext.InspectionRecords.Add(record);  
        await _dbContext.SaveChangesAsync();  
        return record;  
    }  
  
    public async Task UpdateAsync(InspectionRecord record)  
    {  
        _dbContext.InspectionRecords.Update(record);  
        await _dbContext.SaveChangesAsync();  
    }  
  
    public async Task DeleteAsync(InspectionRecord record)  
    {  
        _dbContext.InspectionRecords.Remove(record);  
        await _dbContext.SaveChangesAsync();  
    }  
  
    public async Task<IEnumerable<InspectionRecord>> SearchAsync(string? searchTerm, int page, int pageSize)  
    {  
        var query = _dbContext.InspectionRecords  
            .Include(r => r.Ride)  
            .Include(r => r.Team)  
            .AsQueryable();  
  
        if (!string.IsNullOrEmpty(searchTerm))  
        {  
            query = query.Where(r => r.Ride.RideName.Contains(searchTerm) ||   
                                   r.Team.TeamName.Contains(searchTerm));  
        }  
  
        return await query  
            .Skip((page - 1) * pageSize)  
            .Take(pageSize)  
            .ToListAsync();  
    }  
  
    public async Task<int> CountAsync(string? searchTerm)  
    {  
        var query = _dbContext.InspectionRecords  
            .Include(r => r.Ride)  
            .Include(r => r.Team)  
            .AsQueryable();  
  
        if (!string.IsNullOrEmpty(searchTerm))  
        {  
            query = query.Where(r => r.Ride.RideName.Contains(searchTerm) ||   
                                   r.Team.TeamName.Contains(searchTerm));  
        }  
  
        return await query.CountAsync();  
    }  
  
    public async Task<IEnumerable<InspectionRecord>> SearchByRideAsync(int rideId, int page, int pageSize)  
    {  
        return await _dbContext.InspectionRecords  
            .Where(r => r.RideId == rideId)  
            .Include(r => r.Ride)  
            .Include(r => r.Team)  
            .Skip((page - 1) * pageSize)  
            .Take(pageSize)  
            .ToListAsync();  
    }  
  
    public async Task<int> CountByRideAsync(int rideId)  
    {  
        return await _dbContext.InspectionRecords  
            .Where(r => r.RideId == rideId)  
            .CountAsync();  
    }  
  
    public async Task<object> GetStatsAsync(DateTime? startDate, DateTime? endDate)  
    {  
        var query = _dbContext.InspectionRecords.AsQueryable();  
  
        if (startDate.HasValue)  
        {  
            query = query.Where(r => r.CheckDate >= startDate.Value);  
        }  
  
        if (endDate.HasValue)  
        {  
            query = query.Where(r => r.CheckDate <= endDate.Value);  
        }  
  
        var records = await query.ToListAsync();  
  
        return new  
        {  
            TotalInspections = records.Count,  
            PassedInspections = records.Count(r => r.IsPassed),  
            FailedInspections = records.Count(r => !r.IsPassed),  
            PassRate = records.Any() ? (double)records.Count(r => r.IsPassed) / records.Count * 100 : 0  
        };  
    }  
}