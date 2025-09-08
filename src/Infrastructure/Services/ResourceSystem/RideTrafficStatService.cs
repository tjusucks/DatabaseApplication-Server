using DbApp.Domain.Interfaces.ResourceSystem;

namespace DbApp.Infrastructure.Services.ResourceSystem;

/// <summary>
/// Implementation of ride traffic statistics service.
/// </summary>
public class RideTrafficStatService : IRideTrafficStatService
{
    public Task UpdateTrafficStatsAsync(int rideId, DateTime recordTime)
    {
        throw new NotImplementedException("Traffic stats are managed automatically by the system");
    }

    public Task BatchUpdateTrafficStatsAsync()
    {
        throw new NotImplementedException("Batch updates are triggered by scheduled tasks");
    }

    public Task ArchiveOldStatsAsync(DateTime cutoffDate)
    {
        throw new NotImplementedException("Data archiving is handled by operational tools");
    }
}
