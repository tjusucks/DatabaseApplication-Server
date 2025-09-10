using DbApp.Domain.Entities.UserSystem;

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
    Task<RideEntryRecord?> GetByIdAsync(int entryRecordId);

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
}