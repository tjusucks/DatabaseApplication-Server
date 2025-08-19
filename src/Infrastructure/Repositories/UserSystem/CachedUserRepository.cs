using System.Text.Json;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Microsoft.Extensions.Caching.Distributed;

namespace DbApp.Infrastructure.Repositories.UserSystem;

public class CachedUserRepository(IUserRepository inner, IDistributedCache cache) : IUserRepository
{
    private readonly IUserRepository _inner = inner;
    private readonly IDistributedCache _cache = cache;
    private readonly JsonSerializerOptions _jsonOptions = new();

    public async Task<int> CreateAsync(User user)
    {
        var id = await _inner.CreateAsync(user);
        return id;
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        string key = $"user:{userId}";
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<User>(cached, _jsonOptions);
        }

        var entity = await _inner.GetByIdAsync(userId);
        if (entity != null)
        {
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(entity, _jsonOptions),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        }
        return entity;
    }

    public Task<List<User>> GetAllAsync() => _inner.GetAllAsync();

    public async Task UpdateAsync(User user)
    {
        await _inner.UpdateAsync(user);
        await _cache.RemoveAsync($"user:{user.UserId}");
    }

    public async Task DeleteAsync(User user)
    {
        await _inner.DeleteAsync(user);
        await _cache.RemoveAsync($"user:{user.UserId}");
    }
}
