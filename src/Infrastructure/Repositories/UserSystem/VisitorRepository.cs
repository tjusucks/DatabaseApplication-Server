using DbApp.Application.UserSystem.Visitors.Services;
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

    public async Task AddPointsAsync(int visitorId, int points)
    {
        var visitor = await GetByIdAsync(visitorId);
        if (visitor != null)
        {
            visitor.Points += points;

            // Update member level based on new points
            MembershipService.UpdateMemberLevel(visitor);

            await UpdateAsync(visitor);
        }
    }

    public async Task UpdateVisitorInfoAsync(
        int visitorId,
        string? displayName = null,
        string? phoneNumber = null,
        DateTime? birthDate = null,
        Gender? gender = null,
        VisitorType? visitorType = null,
        int? height = null,
        int? points = null,
        string? memberLevel = null)
    {
        // Clear change tracker to avoid conflicts
        _dbContext.ChangeTracker.Clear();

        // Get entities fresh from database
        var visitor = await _dbContext.Visitors
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId);

        if (visitor == null)
            throw new InvalidOperationException($"Visitor with ID {visitorId} not found");

        // Update user information
        if (displayName != null)
            visitor.User.DisplayName = displayName;
        if (phoneNumber != null)
            visitor.User.PhoneNumber = phoneNumber;
        if (birthDate.HasValue)
            visitor.User.BirthDate = birthDate;
        if (gender.HasValue)
            visitor.User.Gender = gender;

        visitor.User.UpdatedAt = DateTime.UtcNow;

        // Update visitor information
        if (visitorType.HasValue)
            visitor.VisitorType = visitorType.Value;
        if (height.HasValue)
            visitor.Height = height.Value;
        if (points.HasValue)
            visitor.Points = points.Value;
        if (memberLevel != null)
            visitor.MemberLevel = memberLevel;

        visitor.UpdatedAt = DateTime.UtcNow;

        // Save changes
        await _dbContext.SaveChangesAsync();
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

    public async Task<int> GetSearchCountAsync(
        string? keyword = null,
        VisitorType? visitorType = null,
        string? memberLevel = null,
        bool? isBlacklisted = null,
        int? minPoints = null,
        int? maxPoints = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _dbContext.Visitors.Include(v => v.User).AsQueryable();

        // Apply filters
        query = ApplySearchFilters(query, keyword, visitorType, memberLevel,
            isBlacklisted, minPoints, maxPoints, startDate, endDate);

        return await query.CountAsync();
    }

    public async Task<List<Visitor>> SearchWithPaginationAsync(
        string? keyword = null,
        VisitorType? visitorType = null,
        string? memberLevel = null,
        bool? isBlacklisted = null,
        int? minPoints = null,
        int? maxPoints = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = _dbContext.Visitors.Include(v => v.User).AsQueryable();

        // Apply filters
        query = ApplySearchFilters(query, keyword, visitorType, memberLevel,
            isBlacklisted, minPoints, maxPoints, startDate, endDate);

        // Apply pagination
        var skip = (page - 1) * pageSize;
        return await query.Skip(skip).Take(pageSize).ToListAsync();
    }

    private static IQueryable<Visitor> ApplySearchFilters(
        IQueryable<Visitor> query,
        string? keyword = null,
        VisitorType? visitorType = null,
        string? memberLevel = null,
        bool? isBlacklisted = null,
        int? minPoints = null,
        int? maxPoints = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        // Keyword search (name, email, phone)
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(v =>
                v.User.DisplayName.Contains(keyword) ||
                v.User.Email.Contains(keyword) ||
                (v.User.PhoneNumber != null && v.User.PhoneNumber.Contains(keyword)));
        }

        // Filter by visitor type
        if (visitorType.HasValue)
        {
            query = query.Where(v => v.VisitorType == visitorType.Value);
        }

        // Filter by member level
        if (!string.IsNullOrWhiteSpace(memberLevel))
        {
            query = query.Where(v => v.MemberLevel == memberLevel);
        }

        // Filter by blacklist status
        if (isBlacklisted.HasValue)
        {
            query = query.Where(v => v.IsBlacklisted == isBlacklisted.Value);
        }

        // Filter by points range
        if (minPoints.HasValue)
        {
            query = query.Where(v => v.Points >= minPoints.Value);
        }

        if (maxPoints.HasValue)
        {
            query = query.Where(v => v.Points <= maxPoints.Value);
        }

        // Filter by registration date range
        if (startDate.HasValue)
        {
            query = query.Where(v => v.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(v => v.CreatedAt <= endDate.Value);
        }

        return query.OrderByDescending(v => v.CreatedAt);
    }
}
