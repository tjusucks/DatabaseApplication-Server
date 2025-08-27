using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

public class RideTrafficStatRepository(ApplicationDbContext dbContext) : IRideTrafficStatRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task CreateAsync(RideTrafficStat stat)
    {
        _dbContext.RideTrafficStats.Add(stat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<RideTrafficStat?> GetByIdAsync(int rideId, DateTime recordTime)
    {
        return await _dbContext.RideTrafficStats
            .Include(r => r.Ride)
            .FirstOrDefaultAsync(r => r.RideId == rideId && r.RecordTime == recordTime);
    }

    public async Task<List<RideTrafficStat>> GetByRideIdAsync(int rideId)
    {
        return await _dbContext.RideTrafficStats
            .Where(r => r.RideId == rideId)
            .Include(r => r.Ride)
            .OrderByDescending(r => r.RecordTime)
            .ToListAsync();
    }

    public async Task<List<RideTrafficStat>> GetByDateRangeAsync(int rideId, DateTime startDate, DateTime endDate)
    {
        return await _dbContext.RideTrafficStats
            .Where(r => r.RideId == rideId && r.RecordTime >= startDate && r.RecordTime <= endDate)
            .Include(r => r.Ride)
            .OrderBy(r => r.RecordTime)
            .ToListAsync();
    }

    public async Task<List<RideTrafficStat>> GetCrowdedRidesAsync(DateTime? date = null)
    {
        var query = _dbContext.RideTrafficStats
            .Where(r => r.IsCrowded == true);

        if (date.HasValue)
        {
            var startOfDay = date.Value.Date;
            var endOfDay = startOfDay.AddDays(1);
            query = query.Where(r => r.RecordTime >= startOfDay && r.RecordTime < endOfDay);
        }

        return await query
            .Include(r => r.Ride)
            .OrderByDescending(r => r.RecordTime)
            .ToListAsync();
    }

    public async Task<List<RideTrafficStat>> GetPopularityReportAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.RideTrafficStats
            .Where(r => r.RecordTime >= startDate && r.RecordTime <= endDate)
            .Include(r => r.Ride)
            .GroupBy(r => r.RideId)
            .Select(g => g.OrderByDescending(r => r.VisitorCount).First())
            .OrderByDescending(r => r.VisitorCount)
            .ToListAsync();
    }

    public async Task<List<RideTrafficStat>> GetPeakHoursAnalysisAsync(int rideId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _dbContext.RideTrafficStats
            .Where(r => r.RideId == rideId && r.RecordTime >= startOfDay && r.RecordTime < endOfDay)
            .Include(r => r.Ride)
            .OrderBy(r => r.RecordTime)
            .ToListAsync();
    }

    public async Task UpdateAsync(RideTrafficStat stat)
    {
        _dbContext.RideTrafficStats.Update(stat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(RideTrafficStat stat)
    {
        _dbContext.RideTrafficStats.Remove(stat);
        await _dbContext.SaveChangesAsync();
    }
}
