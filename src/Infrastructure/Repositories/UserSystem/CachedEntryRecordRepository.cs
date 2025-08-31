using System.Text.Json;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Microsoft.Extensions.Caching.Distributed;

namespace DbApp.Infrastructure.Repositories.UserSystem;

/// <summary>
/// Cached repository implementation for EntryRecord entity.
/// Provides caching layer over the base EntryRecordRepository.
/// </summary>
public class CachedEntryRecordRepository(IEntryRecordRepository inner, IDistributedCache cache) : IEntryRecordRepository
{
    private readonly IEntryRecordRepository _inner = inner;
    private readonly IDistributedCache _cache = cache;
    private readonly JsonSerializerOptions _jsonOptions = new();

    public async Task<int> CreateAsync(EntryRecord entryRecord)
    {
        var id = await _inner.CreateAsync(entryRecord);
        // Clear visitor-related cache
        await _cache.RemoveAsync($"visitor_entries:{entryRecord.VisitorId}");
        await _cache.RemoveAsync("current_visitors");
        await _cache.RemoveAsync("current_visitor_count");
        return id;
    }

    public async Task<EntryRecord?> GetByIdAsync(int entryRecordId)
    {
        string key = $"entry_record:{entryRecordId}";
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<EntryRecord>(cached, _jsonOptions);
        }

        var entity = await _inner.GetByIdAsync(entryRecordId);
        if (entity != null)
        {
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(entity, _jsonOptions),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        }
        return entity;
    }

    public Task<List<EntryRecord>> GetAllAsync() => _inner.GetAllAsync();

    public async Task<List<EntryRecord>> GetByVisitorIdAsync(int visitorId)
    {
        string key = $"visitor_entries:{visitorId}";
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<List<EntryRecord>>(cached, _jsonOptions) ?? [];
        }

        var entities = await _inner.GetByVisitorIdAsync(visitorId);
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(entities, _jsonOptions),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        return entities;
    }

    public async Task<List<EntryRecord>> GetCurrentVisitorsAsync()
    {
        string key = "current_visitors";
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<List<EntryRecord>>(cached, _jsonOptions) ?? [];
        }

        var entities = await _inner.GetCurrentVisitorsAsync();
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(entities, _jsonOptions),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) });
        return entities;
    }

    public Task<List<EntryRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate) => 
        _inner.GetByDateRangeAsync(startDate, endDate);

    public async Task<int> GetCurrentVisitorCountAsync()
    {
        string key = "current_visitor_count";
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<int>(cached, _jsonOptions);
        }

        var count = await _inner.GetCurrentVisitorCountAsync();
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(count, _jsonOptions),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) });
        return count;
    }

    public Task<(int TotalEntries, int TotalExits, int CurrentCount)> GetDailyStatisticsAsync(DateTime date) => 
        _inner.GetDailyStatisticsAsync(date);

    public async Task UpdateAsync(EntryRecord entryRecord)
    {
        await _inner.UpdateAsync(entryRecord);
        // Clear related cache
        await _cache.RemoveAsync($"entry_record:{entryRecord.EntryRecordId}");
        await _cache.RemoveAsync($"visitor_entries:{entryRecord.VisitorId}");
        await _cache.RemoveAsync("current_visitors");
        await _cache.RemoveAsync("current_visitor_count");
    }

    public async Task DeleteAsync(EntryRecord entryRecord)
    {
        await _inner.DeleteAsync(entryRecord);
        // Clear related cache
        await _cache.RemoveAsync($"entry_record:{entryRecord.EntryRecordId}");
        await _cache.RemoveAsync($"visitor_entries:{entryRecord.VisitorId}");
        await _cache.RemoveAsync("current_visitors");
        await _cache.RemoveAsync("current_visitor_count");
    }

    public Task<List<EntryRecord>> GetByEntryGateAsync(string entryGate) => 
        _inner.GetByEntryGateAsync(entryGate);

    public async Task<EntryRecord?> GetActiveEntryForVisitorAsync(int visitorId)
    {
        string key = $"active_entry:{visitorId}";
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<EntryRecord>(cached, _jsonOptions);
        }

        var entity = await _inner.GetActiveEntryForVisitorAsync(visitorId);
        if (entity != null)
        {
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(entity, _jsonOptions),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        }
        return entity;
    }
}
