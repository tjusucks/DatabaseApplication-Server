using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

public class RideTrafficStatRepository(ApplicationDbContext dbContext) : IRideTrafficStatRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<RideTrafficStat?> GetByIdAsync(int rideId, DateTime recordTime)
    {
        return await _dbContext.RideTrafficStats
            .Include(r => r.Ride)
            .FirstOrDefaultAsync(r => r.RideId == rideId && r.RecordTime == recordTime);
    }

    public async Task<IEnumerable<RideTrafficStat>> SearchAsync(string? searchTerm, int page, int pageSize)
    {
        var query = _dbContext.RideTrafficStats
            .Include(r => r.Ride)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(r => r.Ride.RideName.Contains(searchTerm) ||
                                   r.Ride.Location.Contains(searchTerm));
        }

        return await query
            .OrderByDescending(r => r.RecordTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(string? searchTerm)
    {
        var query = _dbContext.RideTrafficStats
            .Include(r => r.Ride)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(r => r.Ride.RideName.Contains(searchTerm) ||
                                   r.Ride.Location.Contains(searchTerm));
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<RideTrafficStat>> SearchByRideAsync(int rideId, int page, int pageSize)
    {
        return await _dbContext.RideTrafficStats
            .Where(r => r.RideId == rideId)
            .Include(r => r.Ride)
            .OrderByDescending(r => r.RecordTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByRideAsync(int rideId)
    {
        return await _dbContext.RideTrafficStats
            .Where(r => r.RideId == rideId)
            .CountAsync();
    }

    public async Task<IEnumerable<RideTrafficStat>> SearchByCrowdedAsync(bool isCrowded, int page, int pageSize)
    {
        return await _dbContext.RideTrafficStats
            .Where(r => r.IsCrowded == isCrowded)
            .Include(r => r.Ride)
            .OrderByDescending(r => r.RecordTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByCrowdedAsync(bool isCrowded)
    {
        return await _dbContext.RideTrafficStats
            .Where(r => r.IsCrowded == isCrowded)
            .CountAsync();
    }

    public async Task<object> GetStatsAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _dbContext.RideTrafficStats.AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(r => r.RecordTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.RecordTime <= endDate.Value);
        }

        var stats = await query.ToListAsync();

        return new
        {
            TotalRecords = stats.Count,
            CrowdedRecords = stats.Count(r => r.IsCrowded == true),
            AverageVisitorCount = stats.Any() ? stats.Average(r => r.VisitorCount) : 0,
            AverageQueueLength = stats.Any() ? stats.Average(r => r.QueueLength) : 0,
            AverageWaitingTime = stats.Any() ? stats.Average(r => r.WaitingTime) : 0,
            MaxVisitorCount = stats.Any() ? stats.Max(r => r.VisitorCount) : 0,
            MaxQueueLength = stats.Any() ? stats.Max(r => r.QueueLength) : 0,
            MaxWaitingTime = stats.Any() ? stats.Max(r => r.WaitingTime) : 0
        };
    }

    public async Task<RideTrafficStat> AddAsync(RideTrafficStat stat)  
    {  
        _dbContext.RideTrafficStats.Add(stat);  
        await _dbContext.SaveChangesAsync();  
        return stat;  
    }  
  
    public async Task UpdateAsync(RideTrafficStat stat)  
    {  
        stat.UpdatedAt = DateTime.UtcNow;  // 更新时间戳  
        _dbContext.RideTrafficStats.Update(stat);  
        await _dbContext.SaveChangesAsync();  
    }  
  
    public async Task DeleteAsync(RideTrafficStat stat)  
    {  
        _dbContext.RideTrafficStats.Remove(stat);  
        await _dbContext.SaveChangesAsync();  
    }
}