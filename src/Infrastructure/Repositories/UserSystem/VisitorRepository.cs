using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.UserSystem;

/// <summary>
/// Repository implementation for Visitor entity operations.
/// </summary>
public class VisitorRepository(ApplicationDbContext dbContext) : IVisitorRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(Visitor visitor)
    {
        _dbContext.Visitors.Add(visitor);
        await _dbContext.SaveChangesAsync();
        return visitor.VisitorId;
    }

    public async Task<Visitor?> GetByIdAsync(int visitorId)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId);
    }

    public async Task<Visitor?> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.VisitorId == userId);
    }

    public async Task<List<Visitor>> GetAllAsync()
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .ToListAsync();
    }

    public async Task<List<Visitor>> GetByTypeAsync(VisitorType visitorType)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .Where(v => v.VisitorType == visitorType)
            .ToListAsync();
    }

    public async Task<List<Visitor>> GetByMemberLevelAsync(string memberLevel)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .Where(v => v.MemberLevel == memberLevel)
            .ToListAsync();
    }

    public async Task<List<Visitor>> GetByPointsRangeAsync(int minPoints, int maxPoints)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .Where(v => v.Points >= minPoints && v.Points <= maxPoints)
            .ToListAsync();
    }

    public async Task UpdateAsync(Visitor visitor)
    {
        visitor.UpdatedAt = DateTime.UtcNow;
        _dbContext.Visitors.Update(visitor);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Visitor visitor)
    {
        _dbContext.Visitors.Remove(visitor);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddPointsAsync(int visitorId, int points)
    {
        var visitor = await GetByIdAsync(visitorId);
        if (visitor != null)
        {
            visitor.Points += points;
            await UpdateAsync(visitor);
        }
    }

    public async Task<bool> DeductPointsAsync(int visitorId, int points)
    {
        var visitor = await GetByIdAsync(visitorId);
        if (visitor != null && visitor.Points >= points)
        {
            visitor.Points -= points;
            await UpdateAsync(visitor);
            return true;
        }
        return false;
    }
}
