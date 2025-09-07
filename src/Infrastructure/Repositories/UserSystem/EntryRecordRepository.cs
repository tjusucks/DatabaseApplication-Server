using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using DbApp.Domain.Specifications.Common;
using DbApp.Domain.Specifications.UserSystem;
using DbApp.Domain.Statistics.UserSystem;
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

    public async Task<EntryRecord?> GetActiveEntryByVisitorIdAsync(int visitorId)
    {
        return await _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ticket)
            .FirstOrDefaultAsync(er => er.VisitorId == visitorId && er.ExitTime == null);
    }

    public async Task<List<EntryRecord>> SearchAsync(PaginatedSpec<EntryRecordSpec> spec)
    {
        var query = _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .Include(er => er.Ticket)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, spec.InnerSpec);

        // Apply sorting
        query = ApplySorting(query, spec.OrderBy, spec.Descending);

        // Apply pagination
        return await query
            .Skip((spec.Page - 1) * spec.PageSize)
            .Take(spec.PageSize)
            .ToListAsync();
    }

    public async Task<EntryRecordStats> GetStatsAsync(EntryRecordSpec spec)
    {
        var query = _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, spec);

        var totalEntries = await query.CountAsync();
        var totalExits = await query.CountAsync(er => er.ExitTime != null);
        var activeEntries = await query.CountAsync(er => er.ExitTime == null);
        var uniqueVisitors = await query.Select(er => er.VisitorId).Distinct().CountAsync();

        var firstEntry = await query.OrderBy(er => er.EntryTime).FirstOrDefaultAsync();
        var lastEntry = await query.OrderByDescending(er => er.EntryTime).FirstOrDefaultAsync();

        var entryGateCount = await query.Select(er => er.EntryGate).Distinct().CountAsync();
        var exitGateCount = await query.Where(er => er.ExitGate != null).Select(er => er.ExitGate).Distinct().CountAsync();

        return new EntryRecordStats
        {
            TotalEntries = totalEntries,
            TotalExits = totalExits,
            ActiveEntries = activeEntries,
            UniqueVisitors = uniqueVisitors,
            FirstEntryTime = firstEntry?.EntryTime,
            LastEntryTime = lastEntry?.EntryTime,
            EntryGateCount = entryGateCount,
            ExitGateCount = exitGateCount
        };
    }

    public async Task<int> CountAsync(EntryRecordSpec spec)
    {
        var query = _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .AsQueryable();

        query = ApplyFilters(query, spec);
        return await query.CountAsync();
    }

    public async Task<List<GroupedEntryRecordStats>> GetGroupedStatsAsync(GroupedSpec<EntryRecordSpec> spec)
    {
        var query = _dbContext.EntryRecords
            .Include(er => er.Visitor)
            .ThenInclude(v => v.User)
            .AsQueryable();

        query = ApplyFilters(query, spec.InnerSpec);

        // Special handling for date grouping
        if (spec.GroupBy.ToLowerInvariant() == "date")
        {
            var dateGroupedQuery = query.GroupBy(er => er.EntryTime.Date);

            var dateResults = await dateGroupedQuery
                .Select(g => new
                {
                    GroupKey = g.Key, // This will be DateTime (date only)
                    TotalEntries = g.Count(),
                    TotalExits = g.Count(er => er.ExitTime != null),
                    ActiveEntries = g.Count(er => er.ExitTime == null),
                    UniqueVisitors = g.Select(er => er.VisitorId).Distinct().Count(),
                    FirstEntryTime = g.Min(er => er.EntryTime),
                    LastEntryTime = g.Max(er => er.EntryTime),
                    EntryGateCount = g.Select(er => er.EntryGate).Distinct().Count(),
                    ExitGateCount = g.Where(er => er.ExitGate != null).Select(er => er.ExitGate).Distinct().Count()
                })
                .ToListAsync();

            // Convert to final result in memory with string formatting
            return [.. dateResults.Select(r => new GroupedEntryRecordStats
            {
                GroupKey = r.GroupKey.ToString("yyyy-MM-dd"),
                GroupName = r.GroupKey.ToString("yyyy-MM-dd"),
                TotalEntries = r.TotalEntries,
                TotalExits = r.TotalExits,
                ActiveEntries = r.ActiveEntries,
                UniqueVisitors = r.UniqueVisitors,
                FirstEntryTime = r.FirstEntryTime,
                LastEntryTime = r.LastEntryTime,
                EntryGateCount = r.EntryGateCount,
                ExitGateCount = r.ExitGateCount
            })];
        }

        // Handle other grouping types normally
        var groupedQuery = spec.GroupBy.ToLowerInvariant() switch
        {
            "entrygate" => query.GroupBy(er => new
            {
                Key = er.EntryGate,
                Name = er.EntryGate
            }),
            "exitgate" => query.GroupBy(er => new
            {
                Key = er.ExitGate ?? "No Exit",
                Name = er.ExitGate ?? "No Exit"
            }),
            "isactive" => query.GroupBy(er => new
            {
                Key = er.ExitTime == null ? "Active" : "Exited",
                Name = er.ExitTime == null ? "Active" : "Exited"
            }),
            _ => query.GroupBy(er => new
            {
                Key = er.EntryGate,
                Name = er.EntryGate
            })
        };

        var results = await groupedQuery
            .Select(g => new GroupedEntryRecordStats
            {
                GroupKey = g.Key.Key,
                GroupName = g.Key.Name,
                TotalEntries = g.Count(),
                TotalExits = g.Count(er => er.ExitTime != null),
                ActiveEntries = g.Count(er => er.ExitTime == null),
                UniqueVisitors = g.Select(er => er.VisitorId).Distinct().Count(),
                FirstEntryTime = g.Min(er => er.EntryTime),
                LastEntryTime = g.Max(er => er.EntryTime),
                EntryGateCount = g.Select(er => er.EntryGate).Distinct().Count(),
                ExitGateCount = g.Where(er => er.ExitGate != null).Select(er => er.ExitGate).Distinct().Count()
            })
            .ToListAsync();

        return results;
    }

    private static IQueryable<EntryRecord> ApplyFilters(IQueryable<EntryRecord> query, EntryRecordSpec spec)
    {
        if (!string.IsNullOrWhiteSpace(spec.Keyword))
        {
            query = query.Where(er =>
                er.EntryGate.Contains(spec.Keyword) ||
                (er.ExitGate != null && er.ExitGate.Contains(spec.Keyword)) ||
                er.Visitor.User.Username.Contains(spec.Keyword) ||
                er.Visitor.User.DisplayName.Contains(spec.Keyword));
        }

        if (spec.VisitorId.HasValue)
            query = query.Where(er => er.VisitorId == spec.VisitorId);

        if (spec.Start.HasValue)
            query = query.Where(er => er.EntryTime >= spec.Start);

        if (spec.End.HasValue)
            query = query.Where(er => er.EntryTime <= spec.End);

        if (spec.EntryTimeStart.HasValue)
            query = query.Where(er => er.EntryTime >= spec.EntryTimeStart);

        if (spec.EntryTimeEnd.HasValue)
            query = query.Where(er => er.EntryTime <= spec.EntryTimeEnd);

        if (spec.ExitTimeStart.HasValue)
            query = query.Where(er => er.ExitTime >= spec.ExitTimeStart);

        if (spec.ExitTimeEnd.HasValue)
            query = query.Where(er => er.ExitTime <= spec.ExitTimeEnd);

        if (!string.IsNullOrWhiteSpace(spec.EntryGate))
            query = query.Where(er => er.EntryGate == spec.EntryGate);

        if (!string.IsNullOrWhiteSpace(spec.ExitGate))
            query = query.Where(er => er.ExitGate == spec.ExitGate);

        if (spec.TicketId.HasValue)
            query = query.Where(er => er.TicketId == spec.TicketId);

        if (spec.IsActive.HasValue)
        {
            if (spec.IsActive.Value)
                query = query.Where(er => er.ExitTime == null);
            else
                query = query.Where(er => er.ExitTime != null);
        }

        return query;
    }

    private static IQueryable<EntryRecord> ApplySorting(IQueryable<EntryRecord> query, string? sortBy, bool descending)
    {
        var sortKey = sortBy?.Trim().ToLowerInvariant();

        return sortKey switch
        {
            "entrytime" => descending
                ? query.OrderByDescending(er => er.EntryTime)
                : query.OrderBy(er => er.EntryTime),

            "exittime" => descending
                ? query.OrderByDescending(er => er.ExitTime)
                : query.OrderBy(er => er.ExitTime),

            "entrygate" => descending
                ? query.OrderByDescending(er => er.EntryGate)
                : query.OrderBy(er => er.EntryGate),

            "exitgate" => descending
                ? query.OrderByDescending(er => er.ExitGate)
                : query.OrderBy(er => er.ExitGate),

            "visitorname" => descending
                ? query.OrderByDescending(er => er.Visitor.User.DisplayName)
                : query.OrderBy(er => er.Visitor.User.DisplayName),

            "createdat" => descending
                ? query.OrderByDescending(er => er.CreatedAt)
                : query.OrderBy(er => er.CreatedAt),

            _ => descending
                ? query.OrderByDescending(er => er.EntryTime)
                : query.OrderBy(er => er.EntryTime)
        };
    }
}
