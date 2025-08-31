using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
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

    public async Task<Visitor?> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.VisitorId == userId);
    }

    public async Task<List<Visitor>> SearchByNameAsync(string name)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .Where(v => v.User.Username.Contains(name) || v.User.DisplayName.Contains(name))
            .OrderBy(v => v.User.DisplayName)
            .ToListAsync();
    }

    public async Task<List<Visitor>> SearchByPhoneNumberAsync(string phoneNumber)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .Where(v => v.User.PhoneNumber != null && v.User.PhoneNumber.Contains(phoneNumber))
            .OrderBy(v => v.User.DisplayName)
            .ToListAsync();
    }

    public async Task<List<Visitor>> GetByBlacklistStatusAsync(bool isBlacklisted)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .Where(v => v.IsBlacklisted == isBlacklisted)
            .OrderBy(v => v.User.DisplayName)
            .ToListAsync();
    }

    public async Task<List<Visitor>> GetByVisitorTypeAsync(VisitorType visitorType)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .Where(v => v.VisitorType == visitorType)
            .OrderBy(v => v.User.DisplayName)
            .ToListAsync();
    }

    public async Task<List<Visitor>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.Visitors
            .Include(v => v.User)
            .Where(v => v.User.RegisterTime >= startDate && v.User.RegisterTime <= endDate)
            .OrderByDescending(v => v.User.RegisterTime)
            .ToListAsync();
    }

    public async Task<List<Visitor>> SearchAsync(
        string? name = null,
        string? phoneNumber = null,
        bool? isBlacklisted = null,
        VisitorType? visitorType = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _dbContext.Visitors
            .Include(v => v.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(v => v.User.Username.Contains(name) || v.User.DisplayName.Contains(name));
        }

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            query = query.Where(v => v.User.PhoneNumber != null && v.User.PhoneNumber.Contains(phoneNumber));
        }

        if (isBlacklisted.HasValue)
        {
            query = query.Where(v => v.IsBlacklisted == isBlacklisted.Value);
        }

        if (visitorType.HasValue)
        {
            query = query.Where(v => v.VisitorType == visitorType.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(v => v.User.RegisterTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(v => v.User.RegisterTime <= endDate.Value);
        }

        return await query
            .OrderBy(v => v.User.DisplayName)
            .ToListAsync();
    }
}
