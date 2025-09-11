using System.Text.Json;
using System.Text.Json.Serialization;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Microsoft.Extensions.Caching.Distributed;

namespace DbApp.Infrastructure.Services.ResourceSystem;

/// <summary>
/// Service implementation for ride traffic statistics management using distributed cache.
/// </summary>
public class RideTrafficStatService(
    IRideTrafficStatRepository rideTrafficStatRepository,
    IAmusementRideRepository rideRepository,
    IRideEntryRecordRepository rideEntryRecordRepository,
    ApplicationDbContext dbContext,
    IDistributedCache cache) : IRideTrafficStatService
{
    private readonly IRideTrafficStatRepository _rideTrafficStatRepo = rideTrafficStatRepository;
    private readonly IAmusementRideRepository _rideRepo = rideRepository;
    private readonly IRideEntryRecordRepository _rideEntryRecordRepo = rideEntryRecordRepository;
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IDistributedCache _cache = cache;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public async Task<RideTrafficStat?> GetRealTimeStatsAsync(int rideId)
    {
        // Try to get from cache first.
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

        // If not in cache, initialize from database.
        return await InitializeStatsAsync(rideId);
    }

    public async Task<List<RideTrafficStat>> GetAllRealTimeStatsAsync()
    {
        // Get all rides.
        var rides = await _rideRepo.GetAllAsync();

        var stats = new List<RideTrafficStat>();

        // For each ride, get or initialize real-time stats.
        foreach (var ride in rides)
        {
            var stat = await GetRealTimeStatsAsync(ride.RideId);
            if (stat != null)
            {
                stats.Add(stat);
            }
        }

        return stats;
    }

    public async Task UpdateOnRideEntryAsync(int rideId)
    {
        // Get current stats or initialize if not exists.
        var stat = await GetRealTimeStatsAsync(rideId);
        if (stat == null)
        {
            return; // Ride does not exist, nothing to update.
        }

        // Update the statistics.
        stat.VisitorCount++;
        stat.RecordTime = DateTime.UtcNow;
        stat.UpdatedAt = DateTime.UtcNow;

        // Get ride information for accurate calculations.
        var ride = await _rideRepo.GetByIdAsync(rideId);
        if (ride != null)
        {
            // Update queue length estimation.
            // Queue length is the number of people waiting beyond the ride capacity.
            stat.QueueLength = Math.Max(0, stat.VisitorCount - ride.Capacity);

            // Update waiting time estimation based on queue length and ride duration.
            // Waiting time = (queue length / capacity) * ride duration.
            var cyclesNeeded = Math.Ceiling((double)stat.QueueLength / ride.Capacity);
            stat.WaitingTime = (int)(cyclesNeeded * ride.Duration / 60.0); // Convert seconds to minutes.

            // Update crowded status based on capacity utilization.
            // Crowded if more than 2x capacity.
            stat.IsCrowded = stat.VisitorCount > ride.Capacity * 2;
        }

        // Update cache.
        var cacheKey = $"ride_traffic:realtime:{rideId}";
        var statJson = JsonSerializer.Serialize(stat, _jsonOptions);
        await _cache.SetStringAsync(cacheKey, statJson,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
    }

    public async Task UpdateOnRideExitAsync(int rideId)
    {
        // Get current stats or initialize if not exists.
        var stat = await GetRealTimeStatsAsync(rideId);
        if (stat == null)
        {
            return; // Ride does not exist, nothing to update.
        }

        // Update the statistics.
        stat.VisitorCount = Math.Max(0, stat.VisitorCount - 1); // Ensure non-negative.
        stat.UpdatedAt = DateTime.UtcNow;

        // Get ride information for accurate calculations.
        var ride = await _rideRepo.GetByIdAsync(rideId);
        if (ride != null)
        {
            // Update queue length estimation.
            // Queue length is the number of people waiting beyond the ride capacity.
            stat.QueueLength = Math.Max(0, stat.VisitorCount - ride.Capacity);

            // Update waiting time estimation based on queue length and ride duration.
            // Waiting time = (queue length / capacity) * ride duration.
            var cyclesNeeded = Math.Ceiling((double)stat.QueueLength / ride.Capacity);
            stat.WaitingTime = (int)(cyclesNeeded * ride.Duration / 60.0); // Convert seconds to minutes.

            // Update crowded status based on capacity utilization.
            // Crowded if more than 2x capacity.
            stat.IsCrowded = stat.VisitorCount > ride.Capacity * 2;
        }

        // Update cache.
        var cacheKey = $"ride_traffic:realtime:{rideId}";
        var statJson = JsonSerializer.Serialize(stat, _jsonOptions);
        await _cache.SetStringAsync(cacheKey, statJson,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
    }

    public async Task<RideTrafficStat?> InitializeStatsAsync(int rideId)
    {
        // Try to get the latest stats from database.
        var latestStat = await GetLatestDatabaseStatAsync(rideId);

        var statToCache = latestStat ?? await CreateDefaultStatAsync(rideId);

        // Cache the stat.
        var cacheKey = $"ride_traffic:realtime:{rideId}";
        var statJson = JsonSerializer.Serialize(statToCache, _jsonOptions);
        await _cache.SetStringAsync(cacheKey, statJson,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
        return statToCache;
    }

    /// <summary>
    /// Calculates and updates traffic statistics for all rides.
    /// Can be called manually or automatically at scheduled intervals.
    /// </summary>
    /// <param name="recordTime">The time to record the statistics.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task UpdateAllStatsAsync(DateTime recordTime)
    {
        // Get all rides.
        var rides = await _rideRepo.GetAllAsync();

        foreach (var ride in rides)
        {
            // Begin transaction for each ride.
            await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Get the latest database record for this ride.
                var latestStat = await GetLatestDatabaseStatAsync(ride.RideId);

                // Define time window for statistics (last 15 minutes).
                var endTime = recordTime;
                var startTime = latestStat?.RecordTime ?? endTime.AddMinutes(-15);

                // Find stats for this ride, or create default if not found.
                var rideStats = await _rideEntryRecordRepo.GetStatAsync(ride.RideId, startTime, endTime);

                // Create updated statistics.
                var updatedStat = new RideTrafficStat
                {
                    RideId = ride.RideId,
                    RecordTime = recordTime,
                    VisitorCount = latestStat?.VisitorCount ?? 0,
                    QueueLength = 0,
                    WaitingTime = 0,
                    IsCrowded = false,
                    CreatedAt = latestStat?.CreatedAt ?? recordTime,
                    UpdatedAt = recordTime,
                    Ride = ride
                };

                // Update visitor count based on entry/exit stats.
                if (rideStats != null)
                {
                    updatedStat.VisitorCount += rideStats.TotalEntries - rideStats.TotalExits;
                }

                // Update queue length, waiting time and crowded status based on ride capacity.
                updatedStat.QueueLength = Math.Max(0, updatedStat.VisitorCount - ride.Capacity);
                var cyclesNeeded = Math.Ceiling((double)updatedStat.QueueLength / ride.Capacity);
                updatedStat.WaitingTime = (int)(cyclesNeeded * ride.Duration / 60.0); // Convert seconds to minutes.
                updatedStat.IsCrowded = updatedStat.VisitorCount > ride.Capacity * 2;

                // Update cache.
                var cacheKey = $"ride_traffic:realtime:{ride.RideId}";
                var statJson = JsonSerializer.Serialize(updatedStat, _jsonOptions);
                await _cache.SetStringAsync(cacheKey, statJson,
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });

                // Save to database.
                var existingStat = await _rideTrafficStatRepo.GetByIdAsync(ride.RideId, recordTime);
                if (existingStat != null)
                {
                    existingStat.VisitorCount = updatedStat.VisitorCount;
                    existingStat.QueueLength = updatedStat.QueueLength;
                    existingStat.WaitingTime = updatedStat.WaitingTime;
                    existingStat.IsCrowded = updatedStat.IsCrowded;
                    existingStat.UpdatedAt = recordTime;
                    await _rideTrafficStatRepo.UpdateAsync(existingStat);
                }
                else
                {
                    await _rideTrafficStatRepo.AddAsync(updatedStat);
                }

                // Commit transaction for this ride.
                await _dbContext.Database.CommitTransactionAsync();
            }
            catch
            {
                // Rollback transaction for this ride on error.
                await _dbContext.Database.RollbackTransactionAsync();
                throw;
            }
        }
    }

    /// <summary>
    /// Calculates and updates traffic statistics for a specific ride.
    /// Can be called manually or automatically at scheduled intervals.
    /// </summary>
    /// <param name="rideId">The ride ID.</param>
    /// <param name="recordTime">The time to record the statistics.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task UpdateStatAsync(int rideId, DateTime recordTime)
    {
        // Begin database transaction.
        await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Get ride information.
            var ride = await _rideRepo.GetByIdAsync(rideId);
            if (ride == null)
            {
                // Rollback transaction and exit if ride doesn't exist.
                await _dbContext.Database.RollbackTransactionAsync();
                return;
            }

            // Get the latest database record for this ride.
            var latestStat = await GetLatestDatabaseStatAsync(rideId);

            // Define time window for statistics (last 15 minutes).
            var endTime = recordTime;
            var startTime = latestStat?.RecordTime ?? endTime.AddMinutes(-15);

            // Get statistics for this ride in the time window.
            var rideStats = await _rideEntryRecordRepo.GetStatAsync(rideId, startTime, endTime);

            // Create updated statistics.
            var updatedStat = new RideTrafficStat
            {
                RideId = rideId,
                RecordTime = recordTime,
                VisitorCount = latestStat?.VisitorCount ?? 0,
                QueueLength = 0,
                WaitingTime = 0,
                IsCrowded = false,
                CreatedAt = latestStat?.CreatedAt ?? recordTime,
                UpdatedAt = recordTime,
                Ride = ride
            };

            // Update visitor count based on entry/exit stats.
            if (rideStats != null)
            {
                // Add net entries to current visitor count.
                updatedStat.VisitorCount += rideStats.TotalEntries - rideStats.TotalExits;
            }

            // Update queue length, waiting time and crowded status based on ride capacity.
            updatedStat.QueueLength = Math.Max(0, updatedStat.VisitorCount - ride.Capacity);
            var cyclesNeeded = Math.Ceiling((double)updatedStat.QueueLength / ride.Capacity);
            updatedStat.WaitingTime = (int)(cyclesNeeded * ride.Duration / 60.0); // Convert seconds to minutes.
            updatedStat.IsCrowded = updatedStat.VisitorCount > ride.Capacity * 2;

            // Update cache.
            var cacheKey = $"ride_traffic:realtime:{rideId}";
            var statJson = JsonSerializer.Serialize(updatedStat, _jsonOptions);
            await _cache.SetStringAsync(cacheKey, statJson,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });

            // Save to database.
            var existingStat = await _rideTrafficStatRepo.GetByIdAsync(rideId, recordTime);
            if (existingStat != null)
            {
                // Update existing record.
                existingStat.VisitorCount = updatedStat.VisitorCount;
                existingStat.QueueLength = updatedStat.QueueLength;
                existingStat.WaitingTime = updatedStat.WaitingTime;
                existingStat.IsCrowded = updatedStat.IsCrowded;
                existingStat.UpdatedAt = recordTime;
                await _rideTrafficStatRepo.UpdateAsync(existingStat);
            }
            else
            {
                // Add new record.
                await _rideTrafficStatRepo.AddAsync(updatedStat);
            }

            // Commit transaction.
            await _dbContext.Database.CommitTransactionAsync();
        }
        catch
        {
            // Rollback transaction on error.
            await _dbContext.Database.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Retrieves the most recent ride traffic statistics from the database.
    /// If no recent statistics are found, returns null to indicate that default values should be used.
    /// </summary>
    private async Task<RideTrafficStat?> GetLatestDatabaseStatAsync(int rideId)
    {
        try
        {
            // Get stats for the last 15 minutes.
            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddMinutes(-15);

            var stats = await _rideTrafficStatRepo.SearchAsync(
                null, rideId, null, null, null, null, null, null, null, startTime, endTime, true, 1, 1);

            return stats.FirstOrDefault();
        }
        catch
        {
            // If database query fails, return null to use default values.
            return null;
        }
    }

    private async Task<RideTrafficStat?> CreateDefaultStatAsync(int rideId)
    {
        // Get ride information to set the ride name.
        var ride = await _rideRepo.GetByIdAsync(rideId);
        if (ride == null)
        {
            return null; // Ride does not exist.
        }

        return new RideTrafficStat
        {
            RideId = rideId,
            RecordTime = DateTime.UtcNow,
            VisitorCount = 0,
            QueueLength = 0,
            WaitingTime = 0,
            IsCrowded = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Ride = ride
        };
    }
}
