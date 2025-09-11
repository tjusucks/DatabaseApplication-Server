using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using DbApp.Domain.Statistics.UserSystem;
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

    public async Task<RideEntryRecordStats> GetStatAsync(int? rideId, DateTime startTime, DateTime endTime)
    {
        // Build the query based on whether rideId is provided
        var query = _dbContext.RideEntryRecords.AsQueryable();

        if (rideId.HasValue)
        {
            query = query.Where(er => er.RideId == rideId.Value);
        }

        query = query.Where(er => er.UpdatedAt >= startTime && er.UpdatedAt < endTime);

        // Get all relevant records
        var records = await query
            .Include(er => er.Ride)
            .ToListAsync();

        // Calculate statistics
        var entries = records.Where(r => r.EntryTime >= startTime && r.EntryTime < endTime).ToList();
        var exits = records.Where(r => r.ExitTime.HasValue && r.ExitTime >= startTime && r.ExitTime < endTime).ToList();
        var activeEntries = records.Count(r => r.EntryTime >= startTime && r.EntryTime < endTime && (!r.ExitTime.HasValue || r.ExitTime >= endTime));
        var uniqueVisitors = records.Select(r => r.VisitorId).Distinct().Count();

        // Get gate counts
        var entryGateCount = entries.Select(r => r.EntryGate).Distinct().Count();
        var exitGateCount = exits.Select(r => r.ExitGate).Where(g => g != null).Distinct().Count();

        return new RideEntryRecordStats
        {
            RideEntryRecordId = rideId,
            RideName = rideId.HasValue ? records.FirstOrDefault(r => r.RideId == rideId.Value)?.Ride.RideName : null,
            TotalEntries = entries.Count,
            TotalExits = exits.Count,
            ActiveEntries = activeEntries,
            UniqueVisitors = uniqueVisitors,
            FirstEntryTime = entries.MinBy(r => r.EntryTime)?.EntryTime,
            LastEntryTime = entries.MaxBy(r => r.EntryTime)?.EntryTime,
            FirstExitTime = exits.MinBy(r => r.ExitTime)?.ExitTime,
            LastExitTime = exits.MaxBy(r => r.ExitTime)?.ExitTime,
            EntryGateCount = entryGateCount,
            ExitGateCount = exitGateCount,
            StartTime = startTime,
            EndTime = endTime
        };
    }

    public async Task<List<RideEntryRecordStats>> GetAllStatsAsync(DateTime startTime, DateTime endTime)
    {
        // Group records by ride and calculate statistics for each ride
        var rideStats = await _dbContext.RideEntryRecords
            .Include(er => er.Ride)
            .Where(er => er.EntryTime >= startTime && er.EntryTime < endTime)
            .GroupBy(er => new { er.RideId, er.Ride.RideName })
            .Select(g => new RideEntryRecordStats
            {
                RideEntryRecordId = g.Key.RideId,
                RideName = g.Key.RideName,
                TotalEntries = g.Count(r => r.EntryTime >= startTime && r.EntryTime < endTime),
                TotalExits = g.Count(r => r.ExitTime.HasValue && r.ExitTime >= startTime && r.ExitTime < endTime),
                ActiveEntries = g.Count(r => r.EntryTime >= startTime && r.EntryTime < endTime && (!r.ExitTime.HasValue || r.ExitTime >= endTime)),
                UniqueVisitors = g.Select(r => r.VisitorId).Distinct().Count(),
                FirstEntryTime = g.Where(r => r.EntryTime >= startTime && r.EntryTime < endTime).MinBy(r => r.EntryTime)!.EntryTime,
                LastEntryTime = g.Where(r => r.EntryTime >= startTime && r.EntryTime < endTime).MaxBy(r => r.EntryTime)!.EntryTime,
                FirstExitTime = g.Where(r => r.ExitTime.HasValue && r.ExitTime >= startTime && r.ExitTime < endTime)
                    .MinBy(r => r.ExitTime)!.ExitTime,
                LastExitTime = g.Where(r => r.ExitTime.HasValue && r.ExitTime >= startTime && r.ExitTime < endTime)
                    .MaxBy(r => r.ExitTime)!.ExitTime,
                EntryGateCount = g.Where(r => r.EntryTime >= startTime && r.EntryTime < endTime)
                    .Select(r => r.EntryGate).Distinct().Count(),
                ExitGateCount = g.Where(r => r.ExitTime.HasValue && r.ExitTime >= startTime && r.ExitTime < endTime)
                    .Select(r => r.ExitGate).Where(gate => gate != null).Distinct().Count(),
                StartTime = startTime,
                EndTime = endTime
            })
            .ToListAsync();

        return rideStats;
    }
}
