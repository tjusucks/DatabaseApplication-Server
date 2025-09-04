using System.Text.Json;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Microsoft.Extensions.Caching.Distributed;

namespace DbApp.Infrastructure.Repositories.UserSystem;

/// <summary>
<<<<<<< HEAD
/// Cached repository implementation for Visitor entity.
/// Provides caching layer over the base VisitorRepository.
=======
/// Cached decorator for Visitor repository operations.
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
/// </summary>
public class CachedVisitorRepository(IVisitorRepository inner, IDistributedCache cache) : IVisitorRepository
{
    private readonly IVisitorRepository _inner = inner;
    private readonly IDistributedCache _cache = cache;
    private readonly JsonSerializerOptions _jsonOptions = new();

    public async Task<int> CreateAsync(Visitor visitor)
    {
        var id = await _inner.CreateAsync(visitor);
        return id;
    }

    public async Task<Visitor?> GetByIdAsync(int visitorId)
    {
        string key = $"visitor:{visitorId}";
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<Visitor>(cached, _jsonOptions);
        }

        var entity = await _inner.GetByIdAsync(visitorId);
        if (entity != null)
        {
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(entity, _jsonOptions),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        }
        return entity;
    }

<<<<<<< HEAD
    public Task<List<Visitor>> GetAllAsync() => _inner.GetAllAsync();

    public async Task UpdateAsync(Visitor visitor)
    {
        await _inner.UpdateAsync(visitor);
        await _cache.RemoveAsync($"visitor:{visitor.VisitorId}");
    }

    public async Task DeleteAsync(Visitor visitor)
    {
        await _inner.DeleteAsync(visitor);
        await _cache.RemoveAsync($"visitor:{visitor.VisitorId}");
    }

=======
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
    public async Task<Visitor?> GetByUserIdAsync(int userId)
    {
        string key = $"visitor:user:{userId}";
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<Visitor>(cached, _jsonOptions);
        }

        var entity = await _inner.GetByUserIdAsync(userId);
        if (entity != null)
        {
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(entity, _jsonOptions),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        }
        return entity;
    }

<<<<<<< HEAD
    public Task<List<Visitor>> SearchByNameAsync(string name) => _inner.SearchByNameAsync(name);

    public Task<List<Visitor>> SearchByPhoneNumberAsync(string phoneNumber) => _inner.SearchByPhoneNumberAsync(phoneNumber);

    public Task<List<Visitor>> GetByBlacklistStatusAsync(bool isBlacklisted) => _inner.GetByBlacklistStatusAsync(isBlacklisted);

    public Task<List<Visitor>> GetByVisitorTypeAsync(VisitorType visitorType) => _inner.GetByVisitorTypeAsync(visitorType);

    public Task<List<Visitor>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate) =>
        _inner.GetByRegistrationDateRangeAsync(startDate, endDate);

    public Task<List<Visitor>> SearchAsync(
        string? name = null,
        string? phoneNumber = null,
        bool? isBlacklisted = null,
        VisitorType? visitorType = null,
        DateTime? startDate = null,
        DateTime? endDate = null) =>
        _inner.SearchAsync(name, phoneNumber, isBlacklisted, visitorType, startDate, endDate);
=======
    public Task<List<Visitor>> GetAllAsync() => _inner.GetAllAsync();

    public Task<List<Visitor>> GetByTypeAsync(VisitorType visitorType) => _inner.GetByTypeAsync(visitorType);

    public Task<List<Visitor>> GetByMemberLevelAsync(string memberLevel) => _inner.GetByMemberLevelAsync(memberLevel);

    public Task<List<Visitor>> GetByPointsRangeAsync(int minPoints, int maxPoints) => 
        _inner.GetByPointsRangeAsync(minPoints, maxPoints);

    public async Task UpdateAsync(Visitor visitor)
    {
        await _inner.UpdateAsync(visitor);
        await _cache.RemoveAsync($"visitor:{visitor.VisitorId}");
        await _cache.RemoveAsync($"visitor:user:{visitor.VisitorId}");
    }

    public async Task DeleteAsync(Visitor visitor)
    {
        await _inner.DeleteAsync(visitor);
        await _cache.RemoveAsync($"visitor:{visitor.VisitorId}");
        await _cache.RemoveAsync($"visitor:user:{visitor.VisitorId}");
    }

    public async Task AddPointsAsync(int visitorId, int points)
    {
        await _inner.AddPointsAsync(visitorId, points);
        await _cache.RemoveAsync($"visitor:{visitorId}");
        await _cache.RemoveAsync($"visitor:user:{visitorId}");
    }

    public async Task<bool> DeductPointsAsync(int visitorId, int points)
    {
        var result = await _inner.DeductPointsAsync(visitorId, points);
        if (result)
        {
            await _cache.RemoveAsync($"visitor:{visitorId}");
            await _cache.RemoveAsync($"visitor:user:{visitorId}");
        }
        return result;
    }
<<<<<<< HEAD
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
=======

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
        await _inner.UpdateVisitorInfoAsync(visitorId, displayName, phoneNumber, birthDate, gender, visitorType, height, points, memberLevel);

        // Clear cache for this visitor
        await _cache.RemoveAsync($"visitor:{visitorId}");
        await _cache.RemoveAsync($"visitor:user:{visitorId}");
    }
<<<<<<< HEAD
>>>>>>> 2c00408 (feat: implement five core visitor management features)
=======

    public Task<int> GetSearchCountAsync(
        string? keyword = null,
        VisitorType? visitorType = null,
        string? memberLevel = null,
        bool? isBlacklisted = null,
        int? minPoints = null,
        int? maxPoints = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        // Search count is not cached as it changes frequently
        return _inner.GetSearchCountAsync(keyword, visitorType, memberLevel,
            isBlacklisted, minPoints, maxPoints, startDate, endDate);
    }

    public Task<List<Visitor>> SearchWithPaginationAsync(
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
        // Search results are not cached due to complexity and frequent changes
        return _inner.SearchWithPaginationAsync(keyword, visitorType, memberLevel,
            isBlacklisted, minPoints, maxPoints, startDate, endDate, page, pageSize);
    }
>>>>>>> 43d958d (refactor: implement enterprise-grade improvements)
}
