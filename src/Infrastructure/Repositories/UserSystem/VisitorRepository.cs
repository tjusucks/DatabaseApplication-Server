using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.UserSystem;

/// <summary>
/// Repository implementation for Visitor entity.
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

    public async Task<List<Visitor>> GetAllAsync()
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
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
}
