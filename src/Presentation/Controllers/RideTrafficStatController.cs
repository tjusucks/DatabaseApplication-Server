using DbApp.Application.ResourceSystem.RideTrafficStats;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/ride-traffic")]  // 简化路径  
public class RideTrafficStatsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>    
    /// Search ride traffic stats with comprehensive filtering options (read-only).    
    /// </summary>    
    /// <param name="searchTerm">Search term for traffic analysis.</param>    
    /// <param name="rideId">Ride ID filter.</param>  
    /// <param name="isCrowded">Crowded status filter.</param>  
    /// <param name="minVisitorCount">Minimum visitor count filter.</param>  
    /// <param name="maxVisitorCount">Maximum visitor count filter.</param>  
    /// <param name="minQueueLength">Minimum queue length filter.</param>  
    /// <param name="maxQueueLength">Maximum queue length filter.</param>  
    /// <param name="minWaitingTime">Minimum waiting time filter.</param>  
    /// <param name="maxWaitingTime">Maximum waiting time filter.</param>  
    /// <param name="recordTimeFrom">Record time from filter.</param>  
    /// <param name="recordTimeTo">Record time to filter.</param>  
    /// <param name="page">Page number.</param>    
    /// <param name="pageSize">Page size.</param>    
    /// <returns>Paginated ride traffic stat results.</returns>    
    [HttpGet("search")]
    public async Task<ActionResult<RideTrafficStatResult>> Search(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? rideId = null,
        [FromQuery] bool? isCrowded = null,
        [FromQuery] int? minVisitorCount = null,
        [FromQuery] int? maxVisitorCount = null,
        [FromQuery] int? minQueueLength = null,
        [FromQuery] int? maxQueueLength = null,
        [FromQuery] int? minWaitingTime = null,
        [FromQuery] int? maxWaitingTime = null,
        [FromQuery] DateTime? recordTimeFrom = null,
        [FromQuery] DateTime? recordTimeTo = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new SearchRideTrafficStatsQuery(
            searchTerm, rideId, isCrowded, minVisitorCount, maxVisitorCount,
            minQueueLength, maxQueueLength, minWaitingTime, maxWaitingTime,
            recordTimeFrom, recordTimeTo, page, pageSize));
        return Ok(result);
    }

    /// <summary>    
    /// Get ride traffic statistics (read-only).    
    /// </summary>    
    /// <param name="startDate">Start date for statistics.</param>    
    /// <param name="endDate">End date for statistics.</param>    
    /// <param name="rideId">Ride ID filter for stats.</param>  
    /// <returns>Ride traffic statistics.</returns>    
    [HttpGet("stats")]
    public async Task<ActionResult<RideTrafficStatsDto>> GetStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? rideId = null)
    {
        var result = await _mediator.Send(new GetRideTrafficStatsQuery(startDate, endDate, rideId));
        return Ok(result);
    }

    /// <summary>    
    /// Get ride traffic stat by composite key (read-only).    
    /// </summary>    
    /// <param name="rideId">Ride ID.</param>  
    /// <param name="recordTime">Record time.</param>  
    /// <returns>Ride traffic stat details.</returns>    
    [HttpGet("{rideId}/{recordTime}")]
    public async Task<ActionResult<RideTrafficStatSummaryDto>> GetById(int rideId, DateTime recordTime)
    {
        var stat = await _mediator.Send(new GetRideTrafficStatByIdQuery(rideId, recordTime));
        return stat == null ? NotFound() : Ok(stat);
    }
}
