namespace DbApp.Domain.Interfaces.UserSystem;

/// <summary>
/// Service interface for RideEntryRecord operations.
/// Provides methods for visitor ride entry/exit management.
/// </summary>
public interface IRideEntryRecordService
{
    /// <summary>
    /// Creates a new ride entry record for visitor entry to a ride.
    /// </summary>
    Task<int> CreateRideEntryAsync(int visitorId, int rideId, string gateName, int? ticketId);

    /// <summary>
    /// Creates a ride exit record for visitor exit from a ride.
    /// </summary>
    Task<int> CreateRideExitAsync(int visitorId, int rideId, string gateName);
}
