using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

public class AmusementRideRepository(ApplicationDbContext dbContext) : IAmusementRideRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(AmusementRide ride)
    {
        _dbContext.AmusementRides.Add(ride);
        await _dbContext.SaveChangesAsync();
        return ride.RideId;
    }

    public async Task<AmusementRide?> GetByIdAsync(int rideId)
    {
        return await _dbContext.AmusementRides
            .Include(r => r.Manager)
            .Include(r => r.MaintenanceRecords)
            .Include(r => r.InspectionRecords)
            .Include(r => r.RideTrafficStats)
            .FirstOrDefaultAsync(r => r.RideId == rideId);
    }

    public async Task<List<AmusementRide>> GetAllAsync()
    {
        return await _dbContext.AmusementRides
            .Include(r => r.Manager)
            .ToListAsync();
    }

    public async Task<List<AmusementRide>> GetByStatusAsync(RideStatus status)
    {
        return await _dbContext.AmusementRides
            .Where(r => r.RideStatus == status)
            .Include(r => r.Manager)
            .ToListAsync();
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
}
