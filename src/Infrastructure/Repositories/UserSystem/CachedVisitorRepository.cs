using System.Text.Json;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Microsoft.Extensions.Caching.Distributed;

namespace DbApp.Infrastructure.Repositories.UserSystem;

/// <summary>
/// Cached repository implementation for Visitor entity.
/// Provides caching layer over the base VisitorRepository.
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
}
