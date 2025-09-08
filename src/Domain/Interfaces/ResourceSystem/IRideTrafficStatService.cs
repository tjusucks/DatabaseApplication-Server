namespace DbApp.Domain.Interfaces.ResourceSystem;

/// <summary>
/// Background service for automatic ride traffic statistics management.
/// </summary>
public interface IRideTrafficStatService
{
    /// <summary>
    /// Calculate and update traffic statistics based on entry records.
    /// </summary>
    Task UpdateTrafficStatsAsync(int rideId, DateTime recordTime);

    /// <summary>
    /// Batch process traffic statistics for all rides.
    /// </summary>
    Task BatchUpdateTrafficStatsAsync();

    /// <summary>
    /// Archive old traffic statistics data.
    /// </summary>
    Task ArchiveOldStatsAsync(DateTime cutoffDate);
}
