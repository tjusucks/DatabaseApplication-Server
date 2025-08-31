using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

public class MaintenanceRecordRepository(ApplicationDbContext dbContext) : IMaintenanceRecordRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<MaintenanceRecord?> GetByIdAsync(int maintenanceId)
    {
        return await _dbContext.MaintenanceRecords
            .Include(r => r.Ride)
            .Include(r => r.Team)
            .Include(r => r.Manager)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(r => r.MaintenanceId == maintenanceId);
    }

    public async Task<MaintenanceRecord> AddAsync(MaintenanceRecord record)
    {
        _dbContext.MaintenanceRecords.Add(record);
        await _dbContext.SaveChangesAsync();
        return record;
    }

    public async Task UpdateAsync(MaintenanceRecord record)
    {
        _dbContext.MaintenanceRecords.Update(record);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(MaintenanceRecord record)
    {
        _dbContext.MaintenanceRecords.Remove(record);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<MaintenanceRecord>> SearchAsync(string? searchTerm, int page, int pageSize)
    {
        var query = _dbContext.MaintenanceRecords
            .Include(r => r.Ride)
            .Include(r => r.Team)
            .Include(r => r.Manager)
            .ThenInclude(m => m.User)
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
        var query = _dbContext.MaintenanceRecords
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

    public async Task<IEnumerable<MaintenanceRecord>> SearchByRideAsync(int rideId, int page, int pageSize)
    {
        return await _dbContext.MaintenanceRecords
            .Where(r => r.RideId == rideId)
            .Include(r => r.Ride)
            .Include(r => r.Team)
            .Include(r => r.Manager)
            .ThenInclude(m => m.User)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByRideAsync(int rideId)
    {
        return await _dbContext.MaintenanceRecords
            .Where(r => r.RideId == rideId)
            .CountAsync();
    }

    public async Task<IEnumerable<MaintenanceRecord>> SearchByStatusAsync(bool isCompleted, bool? isAccepted, int page, int pageSize)
    {
        var query = _dbContext.MaintenanceRecords
            .Where(r => r.IsCompleted == isCompleted)
            .Include(r => r.Ride)
            .Include(r => r.Team)
            .Include(r => r.Manager)
            .ThenInclude(m => m.User)
            .AsQueryable();

        if (isAccepted.HasValue)
        {
            query = query.Where(r => r.IsAccepted == isAccepted.Value);
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByStatusAsync(bool isCompleted, bool? isAccepted)
    {
        var query = _dbContext.MaintenanceRecords
            .Where(r => r.IsCompleted == isCompleted)
            .AsQueryable();

        if (isAccepted.HasValue)
        {
            query = query.Where(r => r.IsAccepted == isAccepted.Value);
        }

        return await query.CountAsync();
    }

    public async Task<object> GetStatsAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _dbContext.MaintenanceRecords.AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(r => r.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.StartTime <= endDate.Value);
        }

        var records = await query.ToListAsync();

        return new
        {
            TotalMaintenances = records.Count,
            CompletedMaintenances = records.Count(r => r.IsCompleted),
            AcceptedMaintenances = records.Count(r => r.IsAccepted == true),
            TotalCost = records.Sum(r => r.Cost),
            AverageCost = records.Any() ? records.Average(r => r.Cost) : 0
        };
    }
}
