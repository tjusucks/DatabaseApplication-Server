using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

/// <summary>
/// Repository implementation for inspection record operations with unified search capabilities.
/// </summary>
public class InspectionRecordRepository(ApplicationDbContext dbContext) : IInspectionRecordRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    /// <summary>
    /// Get inspection record by ID with related entities.
    /// </summary>
    public async Task<InspectionRecord?> GetByIdAsync(int inspectionId)
    {
        return await _dbContext.InspectionRecords
            .Include(r => r.Ride)
            .Include(r => r.Team)
            .FirstOrDefaultAsync(r => r.InspectionId == inspectionId);
    }

    /// <summary>
    /// Add a new inspection record.
    /// </summary>
    public async Task<InspectionRecord> AddAsync(InspectionRecord record)
    {
        _dbContext.InspectionRecords.Add(record);
        await _dbContext.SaveChangesAsync();
        return record;
    }

    /// <summary>
    /// Update an existing inspection record.
    /// </summary>
    public async Task UpdateAsync(InspectionRecord record)
    {
        _dbContext.InspectionRecords.Update(record);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Delete an inspection record.
    /// </summary>
    public async Task DeleteAsync(InspectionRecord record)
    {
        _dbContext.InspectionRecords.Remove(record);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Unified search method with comprehensive filtering options.
    /// </summary>
    public async Task<IEnumerable<InspectionRecord>> SearchAsync(
        string? keyword,
        int? rideId,
        int? teamId,
        CheckType? checkType,
        bool? isPassed,
        DateTime? checkDateFrom,
        DateTime? checkDateTo,
        int page,
        int pageSize)
    {
        var query = _dbContext.InspectionRecords
            .Include(r => r.Ride)
            .Include(r => r.Team)
            .AsQueryable();

        // Apply all filtering conditions
        query = ApplyFilters(query, keyword, rideId, teamId, checkType,
            isPassed, checkDateFrom, checkDateTo);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Unified count method with comprehensive filtering options.
    /// </summary>
    public async Task<int> CountAsync(
        string? keyword,
        int? rideId,
        int? teamId,
        CheckType? checkType,
        bool? isPassed,
        DateTime? checkDateFrom,
        DateTime? checkDateTo)
    {
        var query = _dbContext.InspectionRecords.AsQueryable();

        query = ApplyFilters(query, keyword, rideId, teamId, checkType,
            isPassed, checkDateFrom, checkDateTo);

        return await query.CountAsync();
    }

    /// <summary>
    /// Get inspection record statistics for a date range.
    /// </summary>
    public async Task<InspectionRecordStats> GetStatsAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _dbContext.InspectionRecords.AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(r => r.CheckDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.CheckDate <= endDate.Value);
        }

        var records = await query.ToListAsync();

        return new InspectionRecordStats
        {
            TotalInspections = records.Count,
            PassedInspections = records.Count(r => r.IsPassed),
            FailedInspections = records.Count(r => !r.IsPassed),
            PassRate = records.Any() ? (double)records.Count(r => r.IsPassed) / records.Count * 100 : 0,
            CheckTypeBreakdown = records.GroupBy(r => r.CheckType)
                .ToDictionary(g => g.Key.ToString(), g => g.Count())
        };
    }

    /// <summary>
    /// Private helper method to apply all filtering conditions.
    /// </summary>
    private static IQueryable<InspectionRecord> ApplyFilters(
        IQueryable<InspectionRecord> query,
        string? keyword,
        int? rideId,
        int? teamId,
        CheckType? checkType,
        bool? isPassed,
        DateTime? checkDateFrom,
        DateTime? checkDateTo)
    {
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(r => r.Ride.RideName.Contains(keyword) ||
                                   r.Team.TeamName.Contains(keyword) ||
                                   (r.IssuesFound != null && r.IssuesFound.Contains(keyword)) ||
                                   (r.Recommendations != null && r.Recommendations.Contains(keyword)));
        }

        if (rideId.HasValue)
        {
            query = query.Where(r => r.RideId == rideId.Value);
        }

        if (teamId.HasValue)
        {
            query = query.Where(r => r.TeamId == teamId.Value);
        }

        if (checkType.HasValue)
        {
            query = query.Where(r => r.CheckType == checkType.Value);
        }

        if (isPassed.HasValue)
        {
            query = query.Where(r => r.IsPassed == isPassed.Value);
        }

        if (checkDateFrom.HasValue)
        {
            query = query.Where(r => r.CheckDate >= checkDateFrom.Value);
        }

        if (checkDateTo.HasValue)
        {
            query = query.Where(r => r.CheckDate <= checkDateTo.Value);
        }

        return query;
    }
}
