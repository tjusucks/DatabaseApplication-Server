using DbApp.Domain.Entities.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

/// <summary>
/// Service interface for ride traffic statistics management.
/// </summary>
public interface IRideTrafficStatService
{
    /// <summary>
    /// Gets real-time traffic statistics for a specific ride.
    /// </summary>
    Task<RideTrafficStat?> GetRealTimeStatsAsync(int rideId);

    /// <summary>
    /// Gets real-time traffic statistics for all rides.
    /// </summary>
    Task<List<RideTrafficStat>> GetAllRealTimeStatsAsync();

    /// <summary>
    /// Updates real-time traffic statistics when a visitor enters a ride.
    /// </summary>
    Task UpdateOnRideEntryAsync(int rideId);

    /// <summary>
    /// Updates real-time traffic statistics when a visitor exits a ride.
    /// </summary>
    Task UpdateOnRideExitAsync(int rideId);

    /// <summary>
    /// Initializes real-time statistics for a ride from the latest database record.
    /// </summary>
    Task<RideTrafficStat?> InitializeStatsAsync(int rideId);

    /// <summary>
    /// Calculates and updates traffic statistics for all rides.
    /// Can be called manually or automatically at scheduled intervals.
    /// </summary>
    /// <param name="recordTime">The time to record the statistics.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task UpdateAllStatsAsync(DateTime recordTime);

    /// <summary>
    /// Calculates and updates traffic statistics for a specific ride.
    /// Can be called manually or automatically at scheduled intervals.
    /// </summary>
    /// <param name="rideId">The ride ID.</param>
    /// <param name="recordTime">The time to record the statistics.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task UpdateStatAsync(int rideId, DateTime recordTime);
}
