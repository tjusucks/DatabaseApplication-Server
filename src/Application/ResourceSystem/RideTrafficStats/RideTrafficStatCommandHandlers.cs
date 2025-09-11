using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.RideTrafficStats;

/// <summary>
/// Handler for ride traffic stat update commands.
/// </summary>
public class RideTrafficStatCommandHandlers(
    IRideTrafficStatService rideTrafficStatService) :
    IRequestHandler<UpdateAllRideTrafficStatsCommand, Unit>,
    IRequestHandler<UpdateRideTrafficStatCommand, Unit>
{
    private readonly IRideTrafficStatService _rideTrafficStatService = rideTrafficStatService;

    /// <summary>
    /// Handle updating traffic statistics for all rides.
    /// </summary>
    public async Task<Unit> Handle(UpdateAllRideTrafficStatsCommand request, CancellationToken cancellationToken)
    {
        await _rideTrafficStatService.UpdateAllStatsAsync(request.RecordTime);
        return Unit.Value;
    }

    /// <summary>
    /// Handle updating traffic statistics for a specific ride.
    /// </summary>
    public async Task<Unit> Handle(UpdateRideTrafficStatCommand request, CancellationToken cancellationToken)
    {
        await _rideTrafficStatService.UpdateStatAsync(request.RideId, request.RecordTime);
        return Unit.Value;
    }
}
