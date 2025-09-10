using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;

namespace DbApp.Infrastructure.Services.ResourceSystem;

/// <summary>
/// Service implementation for ride traffic statistics management using distributed cache.
/// </summary>
public class RideTrafficStatService(
    IRideTrafficStatRepository rideTrafficStatRepository,
    IAmusementRideRepository rideRepository,
    IDistributedCache cache) : IRideTrafficStatService
{
    private readonly IRideTrafficStatRepository _rideTrafficStatRepo = rideTrafficStatRepository;
    private readonly IAmusementRideRepository _rideRepo = rideRepository;
    private readonly IDistributedCache _cache = cache;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<RideTrafficStat> GetRealTimeStatsAsync(int rideId)
    {
        // Try to get from cache first
        var cacheKey = $"ride_traffic:realtime:{rideId}";
        var cachedStatJson = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedStatJson))
        {
            var cachedStat = JsonSerializer.Deserialize<RideTrafficStat>(cachedStatJson, _jsonOptions);
            if (cachedStat != null)
            {
                return cachedStat;
            }
        }

        // If not in cache, initialize from database
        await InitializeStatsAsync(rideId);

        // Return the initialized stat
        var statJson = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(statJson))
        {
            var stat = JsonSerializer.Deserialize<RideTrafficStat>(statJson, _jsonOptions);
            if (stat != null)
            {
                return stat;
            }
        }

        return CreateDefaultStat(rideId);
    }

    public async Task<List<RideTrafficStat>> GetAllRealTimeStatsAsync()
    {
        // Get all rides
        var rides = await _rideRepo.GetAllAsync();

        var stats = new List<RideTrafficStat>();

        // For each ride, get or initialize real-time stats
        foreach (var ride in rides)
        {
            var stat = await GetRealTimeStatsAsync(ride.RideId);
            stats.Add(stat);
        }

        return stats;
    }

    public async Task UpdateOnRideEntryAsync(int rideId)
    {
        // Get current stats or initialize if not exists
        var stat = await GetRealTimeStatsAsync(rideId);

        // Update the statistics
        stat.VisitorCount++;
        stat.UpdatedAt = DateTime.UtcNow;

        // Update queue length and waiting time estimation (simplified logic)
        stat.QueueLength = Math.Max(0, stat.QueueLength - 1); // Assuming one person enters the ride
        stat.WaitingTime = Math.Max(0, stat.VisitorCount * 2); // Simplified: 2 minutes per person

        // Update crowded status (simplified logic)
        var ride = await _rideRepo.GetByIdAsync(rideId);
        if (ride != null)
        {
            stat.IsCrowded = stat.VisitorCount > ride.Capacity * 0.8; // 80% capacity threshold
        }

        // Update cache
        var cacheKey = $"ride_traffic:realtime:{rideId}";
        var statJson = JsonSerializer.Serialize(stat, _jsonOptions);
        await _cache.SetStringAsync(cacheKey, statJson,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
    }

    public async Task UpdateOnRideExitAsync(int rideId)
    {
        // Get current stats or initialize if not exists
        var stat = await GetRealTimeStatsAsync(rideId);

        // Update the statistics
        stat.VisitorCount = Math.Max(0, stat.VisitorCount - 1); // Ensure non-negative
        stat.UpdatedAt = DateTime.UtcNow;

        // Update queue length and waiting time estimation (simplified logic)
        stat.QueueLength++; // Assuming one person exits and joins queue
        stat.WaitingTime = Math.Max(0, stat.VisitorCount * 2); // Simplified: 2 minutes per person

        // Update crowded status (simplified logic)
        var ride = await _rideRepo.GetByIdAsync(rideId);
        if (ride != null)
        {
            stat.IsCrowded = stat.VisitorCount > ride.Capacity * 0.8; // 80% capacity threshold
        }

        // Update cache
        var cacheKey = $"ride_traffic:realtime:{rideId}";
        var statJson = JsonSerializer.Serialize(stat, _jsonOptions);
        await _cache.SetStringAsync(cacheKey, statJson,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
    }

    public async Task InitializeStatsAsync(int rideId)
    {
        // Try to get the latest stats from database
        var latestStat = await GetLatestDatabaseStatAsync(rideId);

        var statToCache = latestStat ?? CreateDefaultStat(rideId);

        // Cache the stat
        var cacheKey = $"ride_traffic:realtime:{rideId}";
        var statJson = JsonSerializer.Serialize(statToCache, _jsonOptions);
        await _cache.SetStringAsync(cacheKey, statJson,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
    }

    /// <summary>
    /// Retrieves the most recent ride traffic statistics from the database.
    /// If no recent statistics are found, returns null to indicate that default values should be used.
    /// </summary>
    private async Task<RideTrafficStat?> GetLatestDatabaseStatAsync(int rideId)
    {
        try
        {
            // Get stats for the last 15 minutes
            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddMinutes(-15);

            var stats = await _rideTrafficStatRepo.SearchAsync(
                null, rideId, null, null, null, null, null, null, null, startTime, endTime, 1, 1);

            return stats.FirstOrDefault();
        }
        catch
        {
            // If database query fails, return null to use default values
            return null;
        }
    }

    private static RideTrafficStat CreateDefaultStat(int rideId)
    {
        return new RideTrafficStat
        {
            RideId = rideId,
            RecordTime = DateTime.UtcNow,
            VisitorCount = 0,
            QueueLength = 0,
            WaitingTime = 0,
            IsCrowded = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
