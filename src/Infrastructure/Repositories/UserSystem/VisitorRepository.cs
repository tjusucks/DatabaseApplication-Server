using System.Text.RegularExpressions;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using DbApp.Domain.Specifications.Common;
using DbApp.Domain.Specifications.UserSystem;
using DbApp.Domain.Statistics.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.UserSystem;

public class VisitorRepository(ApplicationDbContext context) : IVisitorRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<int> CreateAsync(Visitor visitor)
    {
        _context.Users.Add(visitor.User);
        _context.Visitors.Add(visitor);
        await _context.SaveChangesAsync();
        return visitor.VisitorId;
    }

    public async Task<Visitor?> GetByIdAsync(int visitorId)
    {
        return await _context.Visitors
            .Include(v => v.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId);
    }

    public async Task<List<Visitor>> GetAllAsync()
    {
        return await _context.Visitors
            .Include(v => v.User)
            .ThenInclude(u => u.Role)
            .ToListAsync();
    }

    public async Task UpdateAsync(Visitor visitor)
    {
        visitor.User.UpdatedAt = DateTime.UtcNow;
        visitor.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(visitor.User);
        _context.Visitors.Update(visitor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Visitor visitor)
    {
        _context.Visitors.Remove(visitor);
        _context.Users.Remove(visitor.User);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Visitor>> SearchAsync(PaginatedSpec<VisitorSpec> spec)
    {
        var query = _context.Visitors
            .Include(v => v.User)
            .ThenInclude(u => u.Role)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, spec.InnerSpec);

        // Apply sorting
        query = ApplySorting(query, spec.OrderBy, spec.Descending);

        // Apply pagination
        return await query
            .Skip((spec.Page - 1) * spec.PageSize)
            .Take(spec.PageSize)
            .ToListAsync();
    }

    public async Task<VisitorStats> GetStatsAsync(VisitorSpec spec)
    {
        var query = _context.Visitors
            .Include(v => v.User)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, spec);

        var totalVisitors = await query.CountAsync();
        var totalMembers = await query.CountAsync(v => !string.IsNullOrEmpty(v.MemberLevel));
        var blacklistedVisitors = await query.CountAsync(v => v.IsBlacklisted);

        var bronzeMembers = await query.CountAsync(v => v.MemberLevel == "Bronze");
        var silverMembers = await query.CountAsync(v => v.MemberLevel == "Silver");
        var goldMembers = await query.CountAsync(v => v.MemberLevel == "Gold");
        var platinumMembers = await query.CountAsync(v => v.MemberLevel == "Platinum");

        var totalPoints = await query.SumAsync(v => v.Points);
        var averagePoints = totalMembers > 0 ? (double)totalPoints / totalMembers : 0;

        return new VisitorStats
        {
            TotalVisitors = totalVisitors,
            TotalMembers = totalMembers,
            RegularVisitors = totalVisitors - totalMembers,
            BlacklistedVisitors = blacklistedVisitors,
            BronzeMembers = bronzeMembers,
            SilverMembers = silverMembers,
            GoldMembers = goldMembers,
            PlatinumMembers = platinumMembers,
            MembershipRate = totalVisitors > 0 ? (decimal)totalMembers / totalVisitors : 0,
            TotalPointsIssued = totalPoints,
            AveragePointsPerMember = averagePoints
        };
    }

    public async Task<int> CountAsync(VisitorSpec spec)
    {
        var query = _context.Visitors
            .Include(v => v.User)
            .AsQueryable();

        query = ApplyFilters(query, spec);
        return await query.CountAsync();
    }

    public async Task<List<GroupedVisitorStats>> GetGroupedStatsAsync(GroupedSpec<VisitorSpec> spec)
    {
        var query = _context.Visitors
            .Include(v => v.User)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, spec.InnerSpec);

        // Group by specified field using anonymous object with Key and Name
        var groupedQuery = spec.GroupBy.ToLowerInvariant() switch
        {
            "visitortype" => query.GroupBy(v => new
            {
                Key = v.VisitorType.ToString(),
                Name = v.VisitorType.ToString()
            }),
            "memberlevel" => query.GroupBy(v => new
            {
                Key = v.MemberLevel ?? "Regular",
                Name = v.MemberLevel ?? "Regular"
            }),
            "isblacklisted" => query.GroupBy(v => new
            {
                Key = v.IsBlacklisted ? "Blacklisted" : "Active",
                Name = v.IsBlacklisted ? "Blacklisted" : "Active"
            }),
            "gender" => query.GroupBy(v => new
            {
                Key = v.User.Gender.ToString() ?? "Unknown",
                Name = v.User.Gender.ToString() ?? "Unknown"
            }),
            _ => query.GroupBy(v => new
            {
                Key = v.VisitorType.ToString(),
                Name = v.VisitorType.ToString()
            })
        };

        var results = await groupedQuery
            .Select(g => new GroupedVisitorStats
            {
                GroupKey = g.Key.Key,
                GroupName = g.Key.Name,
                TotalVisitors = g.Count(),
                TotalMembers = g.Count(v => !string.IsNullOrEmpty(v.MemberLevel)),
                BlacklistedVisitors = g.Count(v => v.IsBlacklisted),
                TotalPointsIssued = g.Sum(v => v.Points),
                RegularVisitors = g.Count() - g.Count(v => !string.IsNullOrEmpty(v.MemberLevel)),
                MembershipRate = g.Any() ? (decimal)g.Count(v => !string.IsNullOrEmpty(v.MemberLevel)) / g.Count() : 0,
                AveragePointsPerMember = g.Any(v => !string.IsNullOrEmpty(v.MemberLevel))
                    ? (double)g.Sum(v => v.Points) / g.Count(v => !string.IsNullOrEmpty(v.MemberLevel))
                    : 0
            })
            .ToListAsync();

        return results;
    }

    private static IQueryable<Visitor> ApplyFilters(IQueryable<Visitor> query, VisitorSpec spec)
    {
        if (!string.IsNullOrWhiteSpace(spec.Keyword))
        {
            query = query.Where(v =>
                v.User.Username.Contains(spec.Keyword) ||
                v.User.DisplayName.Contains(spec.Keyword) ||
                v.User.Email.Contains(spec.Keyword) ||
                (v.User.PhoneNumber != null && v.User.PhoneNumber.Contains(spec.Keyword)));
        }

        if (spec.UserId.HasValue)
            query = query.Where(v => v.User.UserId == spec.UserId);

        if (spec.BirthDateStart.HasValue)
            query = query.Where(v => v.User.BirthDate >= spec.BirthDateStart);

        if (spec.BirthDateEnd.HasValue)
            query = query.Where(v => v.User.BirthDate <= spec.BirthDateEnd);

        if (spec.Gender.HasValue)
            query = query.Where(v => v.User.Gender == spec.Gender);

        if (spec.RegisterTimeStart.HasValue)
            query = query.Where(v => v.User.RegisterTime >= spec.RegisterTimeStart);

        if (spec.RegisterTimeEnd.HasValue)
            query = query.Where(v => v.User.RegisterTime <= spec.RegisterTimeEnd);

        if (spec.PermissionLevelMin.HasValue)
            query = query.Where(v => v.User.PermissionLevel >= spec.PermissionLevelMin);

        if (spec.PermissionLevelMax.HasValue)
            query = query.Where(v => v.User.PermissionLevel <= spec.PermissionLevelMax);

        if (spec.RoleId.HasValue)
            query = query.Where(v => v.User.RoleId == spec.RoleId);

        if (spec.CreatedAtStart.HasValue)
            query = query.Where(v => v.User.CreatedAt >= spec.CreatedAtStart);

        if (spec.CreatedAtEnd.HasValue)
            query = query.Where(v => v.User.CreatedAt <= spec.CreatedAtEnd);

        if (spec.UpdatedAtStart.HasValue)
            query = query.Where(v => v.User.UpdatedAt >= spec.UpdatedAtStart);

        if (spec.UpdatedAtEnd.HasValue)
            query = query.Where(v => v.User.UpdatedAt <= spec.UpdatedAtEnd);

        if (spec.VisitorType.HasValue)
            query = query.Where(v => v.VisitorType == spec.VisitorType);

        if (spec.PointsMin.HasValue)
            query = query.Where(v => v.Points >= spec.PointsMin);

        if (spec.PointsMax.HasValue)
            query = query.Where(v => v.Points <= spec.PointsMax);

        if (!string.IsNullOrWhiteSpace(spec.MemberLevel))
            query = query.Where(v => v.MemberLevel == spec.MemberLevel);

        if (spec.MemberSinceStart.HasValue)
            query = query.Where(v => v.MemberSince >= spec.MemberSinceStart);

        if (spec.MemberSinceEnd.HasValue)
            query = query.Where(v => v.MemberSince <= spec.MemberSinceEnd);

        if (spec.IsBlacklisted.HasValue)
            query = query.Where(v => v.IsBlacklisted == spec.IsBlacklisted);

        if (spec.HeightMin.HasValue)
            query = query.Where(v => v.Height >= spec.HeightMin);

        if (spec.HeightMax.HasValue)
            query = query.Where(v => v.Height <= spec.HeightMax);

        return query;
    }

    private static IQueryable<Visitor> ApplySorting(IQueryable<Visitor> query, string? orderBy, bool descending)
    {
        var orderKey = orderBy?.Trim().ToLowerInvariant();

        return orderKey switch
        {
            "username" => descending
                ? query.OrderByDescending(v => v.User.Username)
                : query.OrderBy(v => v.User.Username),

            "displayname" => descending
                ? query.OrderByDescending(v => v.User.DisplayName)
                : query.OrderBy(v => v.User.DisplayName),

            "registertime" => descending
                ? query.OrderByDescending(v => v.User.RegisterTime)
                : query.OrderBy(v => v.User.RegisterTime),

            "points" => descending
                ? query.OrderByDescending(v => v.Points)
                : query.OrderBy(v => v.Points),

            "createdat" => descending
                ? query.OrderByDescending(v => v.User.CreatedAt)
                : query.OrderBy(v => v.User.CreatedAt),

            "membersince" => descending
                ? query.OrderByDescending(v => v.MemberSince)
                : query.OrderBy(v => v.MemberSince),

            _ => descending
                ? query.OrderByDescending(v => v.User.RegisterTime)
                : query.OrderBy(v => v.User.RegisterTime)
        };
    }
}
