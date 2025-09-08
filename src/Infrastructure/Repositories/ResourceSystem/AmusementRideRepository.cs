using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

public class AmusementRideRepository(ApplicationDbContext dbContext) : IAmusementRideRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    /// <summary>  
    /// Get amusement ride by ID with manager information.  
    /// </summary>  
    public async Task<AmusementRide?> GetByIdAsync(int rideId)
    {
        return await _dbContext.AmusementRides
            .Include(r => r.Manager)
            .ThenInclude(m => m!.User)
            .FirstOrDefaultAsync(r => r.RideId == rideId);
    }

    /// <summary>  
    /// Add a new amusement ride.  
    /// </summary>  
    public async Task<AmusementRide> AddAsync(AmusementRide ride)
    {
        _dbContext.AmusementRides.Add(ride);
        await _dbContext.SaveChangesAsync();
        return ride;
    }

    /// <summary>  
    /// Update an existing amusement ride.  
    /// </summary>  
    public async Task UpdateAsync(AmusementRide ride)
    {
        _dbContext.AmusementRides.Update(ride);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>  
    /// Delete an amusement ride.  
    /// </summary>  
    public async Task DeleteAsync(AmusementRide ride)
    {
        _dbContext.AmusementRides.Remove(ride);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>  
    /// Unified search method with comprehensive filtering options.  
    /// </summary>  
    public async Task<IEnumerable<AmusementRide>> SearchAsync(
        string? searchTerm,
        RideStatus? status,
        string? location,
        int? managerId,
        int? minCapacity,
        int? maxCapacity,
        int? minHeightLimit,
        int? maxHeightLimit,
        DateTime? openDateFrom,
        DateTime? openDateTo,
        int page,
        int pageSize)
    {
        var query = _dbContext.AmusementRides
            .Include(r => r.Manager)
            .ThenInclude(m => m!.User)
            .AsQueryable();

        // Apply all filtering conditions  
        query = ApplyFilters(query, searchTerm, status, location, managerId,
            minCapacity, maxCapacity, minHeightLimit, maxHeightLimit,
            openDateFrom, openDateTo);

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
        RideStatus? status,
        string? location,
        int? managerId,
        int? minCapacity,
        int? maxCapacity,
        int? minHeightLimit,
        int? maxHeightLimit,
        DateTime? openDateFrom,
        DateTime? openDateTo)
    {
        var query = _dbContext.AmusementRides.AsQueryable();

        query = ApplyFilters(query, searchTerm, status, location, managerId,
            minCapacity, maxCapacity, minHeightLimit, maxHeightLimit,
            openDateFrom, openDateTo);

        return await query.CountAsync();
    }

    /// <summary>  
    /// Get amusement ride statistics for a date range.  
    /// </summary>  
    public async Task<AmusementRideStats> GetStatsAsync(DateTime? startDate, DateTime? endDate)  
{  
    var query = _dbContext.AmusementRides.AsQueryable();  
  
    if (startDate.HasValue)  
    {  
        query = query.Where(r => r.CreatedAt >= startDate.Value);  
    }  
  
    if (endDate.HasValue)  
    {  
        query = query.Where(r => r.CreatedAt <= endDate.Value);  
    }  
  
    var rides = await query.ToListAsync();  
  
    return new AmusementRideStats  
    {  
        TotalRides = rides.Count,  
        OperationalRides = rides.Count(r => r.RideStatus == RideStatus.Operating),  
        MaintenanceRides = rides.Count(r => r.RideStatus == RideStatus.Maintenance),  
        ClosedRides = rides.Count(r => r.RideStatus == RideStatus.Closed),  
        TotalCapacity = rides.Sum(r => r.Capacity),  
        AverageCapacity = rides.Any() ? rides.Average(r => r.Capacity) : 0,  
        AverageDuration = rides.Any() ? rides.Average(r => r.Duration) : 0,  
        FirstOpenDate = rides.Where(r => r.OpenDate.HasValue).Min(r => r.OpenDate),  
        LastOpenDate = rides.Where(r => r.OpenDate.HasValue).Max(r => r.OpenDate)  
    };  
}
    /// <summary>  
    /// Private helper method to apply all filtering conditions.  
    /// </summary>  
    private static IQueryable<AmusementRide> ApplyFilters(
        IQueryable<AmusementRide> query,
        string? searchTerm,
        RideStatus? status,
        string? location,
        int? managerId,
        int? minCapacity,
        int? maxCapacity,
        int? minHeightLimit,
        int? maxHeightLimit,
        DateTime? openDateFrom,
        DateTime? openDateTo)
    {
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(r => r.RideName.Contains(searchTerm) ||
                                   r.Location.Contains(searchTerm) ||
                                   (r.Description != null && r.Description.Contains(searchTerm)));
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.RideStatus == status.Value);
        }

        if (!string.IsNullOrEmpty(location))
        {
            query = query.Where(r => r.Location.Contains(location));
        }

        if (managerId.HasValue)
        {
            query = query.Where(r => r.ManagerId == managerId.Value);
        }

        if (minCapacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= minCapacity.Value);
        }

        if (maxCapacity.HasValue)
        {
            query = query.Where(r => r.Capacity <= maxCapacity.Value);
        }

        if (minHeightLimit.HasValue)
        {
            query = query.Where(r => r.HeightLimitMin >= minHeightLimit.Value);
        }

        if (maxHeightLimit.HasValue)
        {
            query = query.Where(r => r.HeightLimitMax <= maxHeightLimit.Value);
        }

        if (openDateFrom.HasValue)
        {
            query = query.Where(r => r.OpenDate >= openDateFrom.Value);
        }

        if (openDateTo.HasValue)
        {
            query = query.Where(r => r.OpenDate <= openDateTo.Value);
        }

        return query;
    }
}
