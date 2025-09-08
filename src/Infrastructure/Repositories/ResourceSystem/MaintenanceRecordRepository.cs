using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

/// <summary>  
/// Repository implementation for maintenance record operations with unified search capabilities.  
/// </summary>  
public class MaintenanceRecordRepository(ApplicationDbContext dbContext) : IMaintenanceRecordRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    /// <summary>  
    /// Get maintenance record by ID with related entities.  
    /// </summary>  
    public async Task<MaintenanceRecord?> GetByIdAsync(int maintenanceId)
    {
        return await _dbContext.MaintenanceRecords
            .Include(r => r.Ride)
            .Include(r => r.Team)
            .Include(r => r.Manager)
            .ThenInclude(m => m!.User)
            .FirstOrDefaultAsync(r => r.MaintenanceId == maintenanceId);
    }

    /// <summary>  
    /// Add a new maintenance record.  
    /// </summary>  
    public async Task<MaintenanceRecord> AddAsync(MaintenanceRecord record)
    {
        _dbContext.MaintenanceRecords.Add(record);
        await _dbContext.SaveChangesAsync();
        return record;
    }

    /// <summary>  
    /// Update an existing maintenance record.  
    /// </summary>  
    public async Task UpdateAsync(MaintenanceRecord record)
    {
        _dbContext.MaintenanceRecords.Update(record);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>  
    /// Delete a maintenance record.  
    /// </summary>  
    public async Task DeleteAsync(MaintenanceRecord record)
    {
        _dbContext.MaintenanceRecords.Remove(record);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>  
    /// Unified search method with comprehensive filtering options.  
    /// </summary>  
    public async Task<IEnumerable<MaintenanceRecord>> SearchAsync(
        string? searchTerm,
        int? rideId,
        int? teamId,
        int? managerId,
        MaintenanceType? maintenanceType,
        bool? isCompleted,
        bool? isAccepted,
        DateTime? startTimeFrom,
        DateTime? startTimeTo,
        DateTime? endTimeFrom,
        DateTime? endTimeTo,
        decimal? minCost,
        decimal? maxCost,
        int page,
        int pageSize)
    {
        var query = _dbContext.MaintenanceRecords
            .Include(r => r.Ride)
            .Include(r => r.Team)
            .Include(r => r.Manager)
            .ThenInclude(m => m!.User)
            .AsQueryable();

        // Apply all filtering conditions  
        query = ApplyFilters(query, searchTerm, rideId, teamId, managerId,
            maintenanceType, isCompleted, isAccepted, startTimeFrom, startTimeTo,
            endTimeFrom, endTimeTo, minCost, maxCost);

        return await query
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
        int? teamId,
        int? managerId,
        MaintenanceType? maintenanceType,
        bool? isCompleted,
        bool? isAccepted,
        DateTime? startTimeFrom,
        DateTime? startTimeTo,
        DateTime? endTimeFrom,
        DateTime? endTimeTo,
        decimal? minCost,
        decimal? maxCost)
    {
        var query = _dbContext.MaintenanceRecords.AsQueryable();

        query = ApplyFilters(query, searchTerm, rideId, teamId, managerId,
            maintenanceType, isCompleted, isAccepted, startTimeFrom, startTimeTo,
            endTimeFrom, endTimeTo, minCost, maxCost);

        return await query.CountAsync();
    }

    /// <summary>  
    /// Get maintenance record statistics for a date range.  
    /// </summary>  
    public async Task<MaintenanceRecordStats> GetStatsAsync(DateTime? startDate, DateTime? endDate)  
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
  
    return new MaintenanceRecordStats  
    {  
        TotalMaintenances = records.Count,  
        CompletedMaintenances = records.Count(r => r.IsCompleted),  
        AcceptedMaintenances = records.Count(r => r.IsAccepted == true),  
        TotalCost = records.Sum(r => r.Cost),  
        AverageCost = records.Any() ? records.Average(r => r.Cost) : 0,  
        MaintenanceTypeBreakdown = records.GroupBy(r => r.MaintenanceType)  
            .ToDictionary(g => g.Key.ToString(), g => g.Count())  
    };  
}

    /// <summary>  
    /// Private helper method to apply all filtering conditions.  
    /// </summary>  
    private static IQueryable<MaintenanceRecord> ApplyFilters(
        IQueryable<MaintenanceRecord> query,
        string? searchTerm,
        int? rideId,
        int? teamId,
        int? managerId,
        MaintenanceType? maintenanceType,
        bool? isCompleted,
        bool? isAccepted,
        DateTime? startTimeFrom,
        DateTime? startTimeTo,
        DateTime? endTimeFrom,
        DateTime? endTimeTo,
        decimal? minCost,
        decimal? maxCost)
    {
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(r => r.Ride.RideName.Contains(searchTerm) ||
                                   r.Team.TeamName.Contains(searchTerm) ||
                                   (r.PartsReplaced != null && r.PartsReplaced.Contains(searchTerm)) ||
                                   (r.MaintenanceDetails != null && r.MaintenanceDetails.Contains(searchTerm)) ||
                                   (r.AcceptanceComments != null && r.AcceptanceComments.Contains(searchTerm)));
        }

        if (rideId.HasValue)
        {
            query = query.Where(r => r.RideId == rideId.Value);
        }

        if (teamId.HasValue)
        {
            query = query.Where(r => r.TeamId == teamId.Value);
        }

        if (managerId.HasValue)
        {
            query = query.Where(r => r.ManagerId == managerId.Value);
        }

        if (maintenanceType.HasValue)
        {
            query = query.Where(r => r.MaintenanceType == maintenanceType.Value);
        }

        if (isCompleted.HasValue)
        {
            query = query.Where(r => r.IsCompleted == isCompleted.Value);
        }

        if (isAccepted.HasValue)
        {
            query = query.Where(r => r.IsAccepted == isAccepted.Value);
        }

        if (startTimeFrom.HasValue)
        {
            query = query.Where(r => r.StartTime >= startTimeFrom.Value);
        }

        if (startTimeTo.HasValue)
        {
            query = query.Where(r => r.StartTime <= startTimeTo.Value);
        }

        if (endTimeFrom.HasValue)
        {
            query = query.Where(r => r.EndTime >= endTimeFrom.Value);
        }

        if (endTimeTo.HasValue)
        {
            query = query.Where(r => r.EndTime <= endTimeTo.Value);
        }

        if (minCost.HasValue)
        {
            query = query.Where(r => r.Cost >= minCost.Value);
        }

        if (maxCost.HasValue)
        {
            query = query.Where(r => r.Cost <= maxCost.Value);
        }

        return query;
    }
}
