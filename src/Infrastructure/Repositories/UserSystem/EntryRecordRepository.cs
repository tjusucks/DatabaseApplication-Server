using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.UserSystem;

/// <summary>
/// Repository implementation for EntryRecord entity.
/// Provides data access methods for visitor entry/exit management.
/// </summary>
public class EntryRecordRepository(ApplicationDbContext dbContext) : IEntryRecordRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(EntryRecord entryRecord)
    {
        _dbContext.EntryRecords.Add(entryRecord);
        await _dbContext.SaveChangesAsync();
        return entryRecord.EntryRecordId;
    }

    public async Task<EntryRecord?> GetByIdAsync(int entryRecordId)
    {
        return await _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ticket)
            .FirstOrDefaultAsync(er => er.EntryRecordId == entryRecordId);
    }

    public async Task<List<EntryRecord>> GetAllAsync()
    {
        return await _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ticket)
            .OrderByDescending(er => er.EntryTime)
            .ToListAsync();
    }

    public async Task<List<EntryRecord>> GetByVisitorIdAsync(int visitorId)
    {
        return await _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ticket)
            .Where(er => er.VisitorId == visitorId)
            .OrderByDescending(er => er.EntryTime)
            .ToListAsync();
    }

    public async Task<List<EntryRecord>> GetCurrentVisitorsAsync()
    {
        return await _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ticket)
            .Where(er => er.ExitTime == null)
            .OrderByDescending(er => er.EntryTime)
            .ToListAsync();
    }

    public async Task<List<EntryRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ticket)
            .Where(er => er.EntryTime >= startDate && er.EntryTime <= endDate)
            .OrderByDescending(er => er.EntryTime)
            .ToListAsync();
    }

    public async Task<int> GetCurrentVisitorCountAsync()
    {
        return await _dbContext.EntryRecords
            .CountAsync(er => er.ExitTime == null);
    }

    public async Task<(int TotalEntries, int TotalExits, int CurrentCount)> GetDailyStatisticsAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var totalEntries = await _dbContext.EntryRecords
            .CountAsync(er => er.EntryTime >= startOfDay && er.EntryTime < endOfDay);

        var totalExits = await _dbContext.EntryRecords
            .CountAsync(er => er.ExitTime.HasValue && 
                             er.ExitTime >= startOfDay && 
                             er.ExitTime < endOfDay);

        var currentCount = await GetCurrentVisitorCountAsync();

        return (totalEntries, totalExits, currentCount);
    }

    public async Task UpdateAsync(EntryRecord entryRecord)
    {
        entryRecord.UpdatedAt = DateTime.UtcNow;
        _dbContext.EntryRecords.Update(entryRecord);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(EntryRecord entryRecord)
    {
        _dbContext.EntryRecords.Remove(entryRecord);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<EntryRecord>> GetByEntryGateAsync(string entryGate)
    {
        return await _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ticket)
            .Where(er => er.EntryGate == entryGate)
            .OrderByDescending(er => er.EntryTime)
            .ToListAsync();
    }

    public async Task<EntryRecord?> GetActiveEntryForVisitorAsync(int visitorId)
    {
        return await _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ticket)
            .Where(er => er.VisitorId == visitorId && er.ExitTime == null)
            .OrderByDescending(er => er.EntryTime)
            .FirstOrDefaultAsync();
    }
}
