using DbApp.Domain.Entities.ResourceSystem;  
using DbApp.Domain.Enums.ResourceSystem;  
using DbApp.Domain.Interfaces.ResourceSystem;  
using Microsoft.EntityFrameworkCore;  
  
namespace DbApp.Infrastructure.Repositories.ResourceSystem;  
  
public class AmusementRideRepository(ApplicationDbContext dbContext) : IAmusementRideRepository  
{  
    private readonly ApplicationDbContext _dbContext = dbContext;  
  
    public async Task<AmusementRide?> GetByIdAsync(int rideId)  
    {  
        return await _dbContext.AmusementRides  
            .Include(r => r.Manager)  
            .ThenInclude(m => m.User)  
            .FirstOrDefaultAsync(r => r.RideId == rideId);  
    }  
  
    public async Task<AmusementRide> AddAsync(AmusementRide ride)  
    {  
        _dbContext.AmusementRides.Add(ride);  
        await _dbContext.SaveChangesAsync();  
        return ride;  
    }  
  
    public async Task UpdateAsync(AmusementRide ride)  
    {  
        _dbContext.AmusementRides.Update(ride);  
        await _dbContext.SaveChangesAsync();  
    }  
  
    public async Task DeleteAsync(AmusementRide ride)  
    {  
        _dbContext.AmusementRides.Remove(ride);  
        await _dbContext.SaveChangesAsync();  
    }  
  
    public async Task<IEnumerable<AmusementRide>> SearchAsync(string? searchTerm, int page, int pageSize)  
    {  
        var query = _dbContext.AmusementRides  
            .Include(r => r.Manager)  
            .ThenInclude(m => m.User)  
            .AsQueryable();  
  
        if (!string.IsNullOrEmpty(searchTerm))  
        {  
            query = query.Where(r => r.RideName.Contains(searchTerm) ||   
                                   r.Location.Contains(searchTerm));  
        }  
  
        return await query  
            .Skip((page - 1) * pageSize)  
            .Take(pageSize)  
            .ToListAsync();  
    }  
  
    public async Task<int> CountAsync(string? searchTerm)  
    {  
        var query = _dbContext.AmusementRides.AsQueryable();  
  
        if (!string.IsNullOrEmpty(searchTerm))  
        {  
            query = query.Where(r => r.RideName.Contains(searchTerm) ||   
                                   r.Location.Contains(searchTerm));  
        }  
  
        return await query.CountAsync();  
    }  
  
    public async Task<IEnumerable<AmusementRide>> SearchByStatusAsync(RideStatus status, int page, int pageSize)  
    {  
        return await _dbContext.AmusementRides  
            .Where(r => r.RideStatus == status)  
            .Include(r => r.Manager)  
            .ThenInclude(m => m.User)  
            .Skip((page - 1) * pageSize)  
            .Take(pageSize)  
            .ToListAsync();  
    }  
  
    public async Task<int> CountByStatusAsync(RideStatus status)  
    {  
        return await _dbContext.AmusementRides  
            .Where(r => r.RideStatus == status)  
            .CountAsync();  
    }  
  
    public async Task<object> GetStatsAsync(DateTime? startDate, DateTime? endDate)  
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
  
        return new  
        {  
            TotalRides = rides.Count,  
            OperationalRides = rides.Count(r => r.RideStatus == RideStatus.Operating),  
            MaintenanceRides = rides.Count(r => r.RideStatus == RideStatus.Maintenance),  
            ClosedRides = rides.Count(r => r.RideStatus == RideStatus.Closed),  
            TotalCapacity = rides.Sum(r => r.Capacity),  
            AverageCapacity = rides.Any() ? rides.Average(r => r.Capacity) : 0,  
            AverageDuration = rides.Any() ? rides.Average(r => r.Duration) : 0  
        };  
    }  
}