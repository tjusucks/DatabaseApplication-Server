using DbApp.Domain.Entities.ResourceSystem;  
using DbApp.Domain.Interfaces.ResourceSystem;  
using Microsoft.EntityFrameworkCore;  
  
namespace DbApp.Infrastructure.Repositories.ResourceSystem;  
  
public class RideEntryRecordRepository(ApplicationDbContext dbContext) : IRideEntryRecordRepository  
{  
    private readonly ApplicationDbContext _dbContext = dbContext;  
  
    public async Task<RideEntryRecord> CreateAsync(RideEntryRecord record)  
    {  
        _dbContext.RideEntryRecords.Add(record);  
        await _dbContext.SaveChangesAsync();  
        return record;  
    }  
  
    public async Task<RideEntryRecord?> GetByIdAsync(int id)  
    {  
        return await _dbContext.RideEntryRecords  
            .Include(r => r.Ride)  
            .Include(r => r.Visitor)  
            .FirstOrDefaultAsync(r => r.RideEntryRecordId == id);  
    }  
  
    public async Task<List<RideEntryRecord>> GetByRideIdAsync(int rideId)  
    {  
        return await _dbContext.RideEntryRecords  
            .Include(r => r.Visitor)  
            .Where(r => r.RideId == rideId)  
            .ToListAsync();  
    }  
  
    public async Task<List<RideEntryRecord>> GetByVisitorIdAsync(int visitorId)  
    {  
        return await _dbContext.RideEntryRecords  
            .Include(r => r.Ride)  
            .Where(r => r.VisitorId == visitorId)  
            .ToListAsync();  
    }  
  
    public async Task<List<RideEntryRecord>> GetFilteredAsync(int? rideId, int? visitorId, DateTime? startDate, DateTime? endDate, int page, int pageSize)  
    {  
        var query = _dbContext.RideEntryRecords  
            .Include(r => r.Ride)  
            .Include(r => r.Visitor)  
            .AsQueryable();  
  
        if (rideId.HasValue)  
            query = query.Where(r => r.RideId == rideId.Value);  
  
        if (visitorId.HasValue)  
            query = query.Where(r => r.VisitorId == visitorId.Value);  
  
        if (startDate.HasValue)  
            query = query.Where(r => r.EntryTime >= startDate.Value);  
  
        if (endDate.HasValue)  
            query = query.Where(r => r.EntryTime <= endDate.Value);  
  
        return await query  
            .Skip((page - 1) * pageSize)  
            .Take(pageSize)  
            .ToListAsync();  
    }  
  
    public async Task<List<RideEntryRecord>> GetCurrentVisitorsAsync()  
    {  
        return await _dbContext.RideEntryRecords  
            .Include(r => r.Ride)  
            .Include(r => r.Visitor)  
            .Where(r => r.ExitTime == null)  
            .ToListAsync();  
    }  
  
    public async Task<object> GetTrafficSummaryAsync(DateTime startDate, DateTime endDate, int? rideId)  
{  
    var query = _dbContext.RideEntryRecords.AsQueryable();  
  
    if (rideId.HasValue)  
        query = query.Where(r => r.RideId == rideId.Value);  
  
    query = query.Where(r => r.EntryTime >= startDate && r.EntryTime <= endDate);  
  
    var summary = await query  
        .GroupBy(r => r.EntryTime.Date)  
        .Select(g => new  
        {  
            Date = g.Key,  
            TotalVisitors = g.Count(),  
            AverageStayTime = g.Where(r => r.ExitTime.HasValue)  
                .Average(r => (r.ExitTime.Value - r.EntryTime).TotalMinutes)  
        })  
        .ToListAsync();  
  
    return summary;  
}
  
    public async Task UpdateAsync(RideEntryRecord record)  
    {  
        _dbContext.RideEntryRecords.Update(record);  
        await _dbContext.SaveChangesAsync();  
    }  
  
    public async Task DeleteAsync(RideEntryRecord record)  
    {  
        _dbContext.RideEntryRecords.Remove(record);  
        await _dbContext.SaveChangesAsync();  
    }  
}