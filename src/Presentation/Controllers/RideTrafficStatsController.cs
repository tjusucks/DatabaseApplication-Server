using DbApp.Application.ResourceSystem.RideTrafficStats;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/ride-traffic-stats")]
public class RideTrafficStatsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>  
    /// Search ride traffic stats by ride ID (read-only).  
    /// </summary>  
    [HttpGet("ride/{rideId}/search")]
    public async Task<ActionResult<RideTrafficStatResult>> SearchByRide(
        [FromRoute] int rideId,
        [FromQuery] SearchRideTrafficStatsByRideQuery query)
    {
        var queryWithRideId = query with { RideId = rideId };
        var result = await _mediator.Send(queryWithRideId);
        return Ok(result);
    }

    /// <summary>  
    /// Get ride traffic statistics (read-only).  
    /// </summary>  
    [HttpGet("stats/search")]
    public async Task<ActionResult<RideTrafficStatsDto>> GetStats(
        [FromQuery] GetRideTrafficStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>  
    /// Get ride traffic stat by composite key (read-only).  
    /// </summary>  
    [HttpGet("{rideId}/{recordTime}")]
    public async Task<ActionResult<RideTrafficStatSummaryDto>> GetById(int rideId, DateTime recordTime)
    {
        var stat = await _mediator.Send(new GetRideTrafficStatByIdQuery(rideId, recordTime));
        return stat == null ? NotFound() : Ok(stat);
    }
}
