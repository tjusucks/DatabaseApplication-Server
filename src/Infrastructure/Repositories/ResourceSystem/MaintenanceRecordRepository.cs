using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

public class MaintenanceRecordRepository(ApplicationDbContext dbContext) : IMaintenanceRecordRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(MaintenanceRecord record)
    {
        _dbContext.MaintenanceRecords.Add(record);
        await _dbContext.SaveChangesAsync();
        return record.MaintenanceId;
    }

    public async Task<MaintenanceRecord?> GetByIdAsync(int maintenanceId)
    {
        return await _dbContext.MaintenanceRecords
            .Include(m => m.Ride)
            .Include(m => m.Team)
            .Include(m => m.Manager)
            .FirstOrDefaultAsync(m => m.MaintenanceId == maintenanceId);
    }

    public async Task<List<MaintenanceRecord>> GetAllAsync()
    {
        return await _dbContext.MaintenanceRecords
            .Include(m => m.Ride)
            .Include(m => m.Team)
            .Include(m => m.Manager)
            .OrderByDescending(m => m.StartTime)
            .ToListAsync();
    }

    public async Task<List<MaintenanceRecord>> GetByRideIdAsync(int rideId)
    {
        return await _dbContext.MaintenanceRecords
            .Where(m => m.RideId == rideId)
            .Include(m => m.Team)
            .Include(m => m.Manager)
            .OrderByDescending(m => m.StartTime)
            .ToListAsync();
    }

    public async Task<List<MaintenanceRecord>> GetPendingAsync()
    {
        return await _dbContext.MaintenanceRecords
            .Where(m => !m.IsCompleted)
            .Include(m => m.Ride)
            .Include(m => m.Team)
            .Include(m => m.Manager)
            .OrderBy(m => m.StartTime)
            .ToListAsync();
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
}
