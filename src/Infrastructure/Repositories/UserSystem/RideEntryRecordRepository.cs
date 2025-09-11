using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.UserSystem;

/// <summary>
/// Repository implementation for RideEntryRecord entity.
/// Provides data access methods for visitor ride entry/exit management.
/// </summary>
public class RideEntryRecordRepository(ApplicationDbContext dbContext) : IRideEntryRecordRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(RideEntryRecord rideEntryRecord)
    {
        _dbContext.RideEntryRecords.Add(rideEntryRecord);
        await _dbContext.SaveChangesAsync();
        return rideEntryRecord.RideEntryRecordId;
    }

    public async Task<RideEntryRecord?> GetByIdAsync(int rideEntryRecordId)
    {
        return await _dbContext.RideEntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ride)
            .Include(er => er.Ticket)
            .FirstOrDefaultAsync(er => er.RideEntryRecordId == rideEntryRecordId);
    }

    public async Task<List<RideEntryRecord>> GetAllAsync()
    {
        return await _dbContext.RideEntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ride)
            .Include(er => er.Ticket)
            .OrderByDescending(er => er.EntryTime)
            .ToListAsync();
    }

    public async Task UpdateAsync(RideEntryRecord rideEntryRecord)
    {
        rideEntryRecord.UpdatedAt = DateTime.UtcNow;
        _dbContext.RideEntryRecords.Update(rideEntryRecord);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(RideEntryRecord rideEntryRecord)
    {
        _dbContext.RideEntryRecords.Remove(rideEntryRecord);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<RideEntryRecord?> GetActiveEntry(int visitorId, int rideId)
    {
        return await _dbContext.RideEntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ride)
            .Include(er => er.Ticket)
            .FirstOrDefaultAsync(er => er.VisitorId == visitorId && er.RideId == rideId && er.ExitTime == null);
    }
}
