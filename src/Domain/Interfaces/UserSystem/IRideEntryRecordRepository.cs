using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Statistics.UserSystem;

namespace DbApp.Domain.Interfaces.UserSystem;

/// <summary>
/// Repository interface for RideEntryRecord entity operations.
/// Provides basic CRUD methods for visitor ride entry/exit management.
/// </summary>
public interface IRideEntryRecordRepository
{
    /// <summary>
    /// Creates a new ride entry record for visitor entry to a ride.
    /// </summary>
    Task<int> CreateAsync(RideEntryRecord rideEntryRecord);

    /// <summary>
    /// Gets a ride entry record by its ID.
    /// </summary>
    Task<RideEntryRecord?> GetByIdAsync(int rideEntryRecordId);

    /// <summary>
    /// Gets all ride entry records.
    /// </summary>
    Task<List<RideEntryRecord>> GetAllAsync();

    /// <summary>
    /// Updates a ride entry record (typically for exit registration).
    /// </summary>
    Task UpdateAsync(RideEntryRecord rideEntryRecord);

    /// <summary>
    /// Deletes a ride entry record.
    /// </summary>
    Task DeleteAsync(RideEntryRecord rideEntryRecord);

    /// <summary>
    /// Gets the active (not exited) ride entry record for a given visitor and ride.
    /// </summary>
    Task<RideEntryRecord?> GetActiveEntry(int visitorId, int rideId);

    /// <summary>
    /// Gets statistics for ride entry records within a time range for a specific ride.
    /// </summary>
    /// <param name="rideId">The ride ID (optional).</param>
    /// <param name="startTime">Start time (inclusive).</param>
    /// <param name="endTime">End time (exclusive).</param>
    /// <returns>Ride entry record statistics.</returns>
    Task<RideEntryRecordStats> GetStatAsync(int? rideId, DateTime startTime, DateTime endTime);

    /// <summary>
    /// Gets statistics for ride entry records within a time range for all rides.
    /// </summary>
    /// <param name="startTime">Start time (inclusive).</param>
    /// <param name="endTime">End time (exclusive).</param>
    /// <returns>List of ride entry record statistics grouped by ride.</returns>
    Task<List<RideEntryRecordStats>> GetAllStatsAsync(DateTime startTime, DateTime endTime);
}
