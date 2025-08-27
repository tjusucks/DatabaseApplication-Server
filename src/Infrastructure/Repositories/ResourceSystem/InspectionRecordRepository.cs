using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

public class InspectionRecordRepository(ApplicationDbContext dbContext) : IInspectionRecordRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(InspectionRecord record)
    {
        _dbContext.InspectionRecords.Add(record);
        await _dbContext.SaveChangesAsync();
        return record.InspectionId;
    }

    public async Task<InspectionRecord?> GetByIdAsync(int inspectionId)
    {
        return await _dbContext.InspectionRecords
            .Include(i => i.Ride)
            .Include(i => i.Team)
            .FirstOrDefaultAsync(i => i.InspectionId == inspectionId);
    }

    public async Task<List<InspectionRecord>> GetAllAsync()
    {
        return await _dbContext.InspectionRecords
            .Include(i => i.Ride)
            .Include(i => i.Team)
            .OrderByDescending(i => i.CheckDate)
            .ToListAsync();
    }

    public async Task<List<InspectionRecord>> GetByRideIdAsync(int rideId)
    {
        return await _dbContext.InspectionRecords
            .Where(i => i.RideId == rideId)
            .Include(i => i.Team)
            .OrderByDescending(i => i.CheckDate)
            .ToListAsync();
    }

    public async Task<List<InspectionRecord>> GetByTeamIdAsync(int teamId)
    {
        return await _dbContext.InspectionRecords
            .Where(i => i.TeamId == teamId)
            .Include(i => i.Ride)
            .OrderByDescending(i => i.CheckDate)
            .ToListAsync();
    }

    public async Task<List<InspectionRecord>> GetFailedInspectionsAsync()
    {
        return await _dbContext.InspectionRecords
            .Where(i => !i.IsPassed)
            .Include(i => i.Ride)
            .Include(i => i.Team)
            .OrderByDescending(i => i.CheckDate)
            .ToListAsync();
    }

    public async Task<List<InspectionRecord>> GetByCheckTypeAsync(CheckType checkType)
    {
        return await _dbContext.InspectionRecords
            .Where(i => i.CheckType == checkType)
            .Include(i => i.Ride)
            .Include(i => i.Team)
            .OrderByDescending(i => i.CheckDate)
            .ToListAsync();
    }

    public async Task<List<InspectionRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.InspectionRecords
            .Where(i => i.CheckDate >= startDate && i.CheckDate <= endDate)
            .Include(i => i.Ride)
            .Include(i => i.Team)
            .OrderByDescending(i => i.CheckDate)
            .ToListAsync();
    }

    public async Task UpdateAsync(InspectionRecord record)
    {
        _dbContext.InspectionRecords.Update(record);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(InspectionRecord record)
    {
        _dbContext.InspectionRecords.Remove(record);
        await _dbContext.SaveChangesAsync();
    }
}
