using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

/// <summary>  
/// Repository implementation for ride traffic stat operations with unified search capabilities.  
/// </summary>  
public class RideTrafficStatRepository(ApplicationDbContext dbContext) : IRideTrafficStatRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    /// <summary>  
    /// Get ride traffic stat by composite key with related entities.  
    /// </summary>  
    public async Task<RideTrafficStat?> GetByIdAsync(int rideId, DateTime recordTime)
    {
        return await _dbContext.RideTrafficStats
            .Include(r => r.Ride)
            .FirstOrDefaultAsync(r => r.RideId == rideId && r.RecordTime == recordTime);
    }

    /// <summary>  
    /// Add a new ride traffic stat.  
    /// </summary>  
    public async Task<RideTrafficStat> AddAsync(RideTrafficStat stat)
    {
        _dbContext.RideTrafficStats.Add(stat);
        await _dbContext.SaveChangesAsync();
        return stat;
    }

    /// <summary>  
    /// Update an existing ride traffic stat.  
    /// </summary>  
    public async Task UpdateAsync(RideTrafficStat stat)
    {
        stat.UpdatedAt = DateTime.UtcNow;
        _dbContext.RideTrafficStats.Update(stat);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>  
    /// Delete a ride traffic stat.  
    /// </summary>  
    public async Task DeleteAsync(RideTrafficStat stat)
    {
        _dbContext.RideTrafficStats.Remove(stat);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>  
    /// Unified search method with comprehensive filtering options.  
    /// </summary>  
    public async Task<IEnumerable<RideTrafficStat>> SearchAsync(
        string? searchTerm,
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
        int pageSize)
    {
        var query = _dbContext.RideTrafficStats
            .Include(r => r.Ride)
            .AsQueryable();

        // Apply all filtering conditions  
        query = ApplyFilters(query, searchTerm, rideId, isCrowded,
            minVisitorCount, maxVisitorCount, minQueueLength, maxQueueLength,
            minWaitingTime, maxWaitingTime, recordTimeFrom, recordTimeTo);

        return await query
            .OrderByDescending(r => r.RecordTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>  
    /// Unified count method with comprehensive filtering options.  
    /// </summary>  
    public async Task<int> CountAsync(
        string? searchTerm,
        int? rideId,
        bool? isCrowded,
        int? minVisitorCount,
        int? maxVisitorCount,
        int? minQueueLength,
        int? maxQueueLength,
        int? minWaitingTime,
        int? maxWaitingTime,
        DateTime? recordTimeFrom,
        DateTime? recordTimeTo)
    {
        var query = _dbContext.RideTrafficStats.AsQueryable();

        query = ApplyFilters(query, searchTerm, rideId, isCrowded,
            minVisitorCount, maxVisitorCount, minQueueLength, maxQueueLength,
            minWaitingTime, maxWaitingTime, recordTimeFrom, recordTimeTo);

        return await query.CountAsync();
    }

    /// <summary>  
    /// Get ride traffic statistics for a date range.  
    /// </summary>  
    public async Task<RideTrafficStats> GetStatsAsync(DateTime? startDate, DateTime? endDate)  
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
  
    return new RideTrafficStats  
    {  
        TotalRecords = stats.Count,  
        CrowdedRecords = stats.Count(r => r.IsCrowded == true),  
        AverageVisitorCount = stats.Any() ? stats.Average(r => r.VisitorCount) : 0,  
        AverageQueueLength = stats.Any() ? stats.Average(r => r.QueueLength) : 0,  
        AverageWaitingTime = stats.Any() ? stats.Average(r => r.WaitingTime) : 0,  
        MaxVisitorCount = stats.Any() ? stats.Max(r => r.VisitorCount) : 0,  
        MaxQueueLength = stats.Any() ? stats.Max(r => r.QueueLength) : 0,  
        MaxWaitingTime = stats.Any() ? stats.Max(r => r.WaitingTime) : 0,  
        CrowdedPercentage = stats.Any() ? (double)stats.Count(r => r.IsCrowded == true) / stats.Count * 100 : 0  
    };  
}
    /// <summary>  
    /// Private helper method to apply all filtering conditions.  
    /// </summary>  
    private static IQueryable<RideTrafficStat> ApplyFilters(
        IQueryable<RideTrafficStat> query,
        string? searchTerm,
        int? rideId,
        bool? isCrowded,
        int? minVisitorCount,
        int? maxVisitorCount,
        int? minQueueLength,
        int? maxQueueLength,
        int? minWaitingTime,
        int? maxWaitingTime,
        DateTime? recordTimeFrom,
        DateTime? recordTimeTo)
    {
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(r => r.Ride.RideName.Contains(searchTerm) ||
                                   r.Ride.Location.Contains(searchTerm));
        }

        if (rideId.HasValue)
        {
            query = query.Where(r => r.RideId == rideId.Value);
        }

        if (isCrowded.HasValue)
        {
            query = query.Where(r => r.IsCrowded == isCrowded.Value);
        }

        if (minVisitorCount.HasValue)
        {
            query = query.Where(r => r.VisitorCount >= minVisitorCount.Value);
        }

        if (maxVisitorCount.HasValue)
        {
            query = query.Where(r => r.VisitorCount <= maxVisitorCount.Value);
        }

        if (minQueueLength.HasValue)
        {
            query = query.Where(r => r.QueueLength >= minQueueLength.Value);
        }

        if (maxQueueLength.HasValue)
        {
            query = query.Where(r => r.QueueLength <= maxQueueLength.Value);
        }

        if (minWaitingTime.HasValue)
        {
            query = query.Where(r => r.WaitingTime >= minWaitingTime.Value);
        }

        if (maxWaitingTime.HasValue)
        {
            query = query.Where(r => r.WaitingTime <= maxWaitingTime.Value);
        }

        if (recordTimeFrom.HasValue)
        {
            query = query.Where(r => r.RecordTime >= recordTimeFrom.Value);
        }

        if (recordTimeTo.HasValue)
        {
            query = query.Where(r => r.RecordTime <= recordTimeTo.Value);
        }

        return query;
    }
}
