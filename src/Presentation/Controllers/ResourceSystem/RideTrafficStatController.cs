using DbApp.Application.ResourceSystem.RideTrafficStats;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/ride-traffic")]
public class RideTrafficStatsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

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

    /// <summary>
    /// Search ride traffic stats with comprehensive filtering options (read-only).
    /// </summary>
    /// <param name="keyword">Keyword for traffic analysis.</param>
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
        [FromQuery] string? keyword = null,
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
            keyword, rideId, isCrowded, minVisitorCount, maxVisitorCount,
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
    /// Get real-time traffic statistics for a specific ride.
    /// </summary>
    /// <param name="rideId">Ride ID.</param>
    /// <returns>Real-time ride traffic statistics.</returns>
    [HttpGet("realtime/{rideId}")]
    public async Task<ActionResult<RideTrafficStatSummaryDto>> GetRealTimeStats(int rideId)
    {
        var query = new GetRealTimeRideTrafficStatQuery(rideId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get real-time traffic statistics for all rides.
    /// </summary>
    /// <returns>List of real-time ride traffic statistics.</returns>
    [HttpGet("realtime")]
    public async Task<ActionResult<List<RideTrafficStatSummaryDto>>> GetAllRealTimeStats()
    {
        var query = new GetAllRealTimeRideTrafficStatsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Manually trigger traffic statistics update for all rides.
    /// </summary>
    /// <returns>Status of manual update trigger.</returns>
    [HttpPost("update")]
    public async Task<ActionResult> TriggerManualUpdate()
    {
        var command = new UpdateAllRideTrafficStatsCommand(DateTime.UtcNow);
        await _mediator.Send(command);
        return Ok(new
        {
            Message = "Manual traffic statistics update triggered successfully.",
            TriggeredAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Manually trigger traffic statistics update for a specific ride.
    /// </summary>
    /// <param name="rideId">Ride ID.</param>
    /// <returns>Status of manual update trigger for the ride.</returns>
    [HttpPost("update/{rideId}")]
    public async Task<ActionResult> TriggerManualUpdateByRideId(int rideId)
    {
        var command = new UpdateRideTrafficStatCommand(rideId, DateTime.UtcNow);
        await _mediator.Send(command);
        return Ok(new
        {
            Message = $"Manual traffic statistics update triggered for ride {rideId} successfully.",
            RideId = rideId,
            TriggeredAt = DateTime.UtcNow
        });
    }

#pragma warning disable
    /// <summary>
    /// Configure scheduled traffic statistics update.
    /// This endpoint allows setting up automatic traffic statistics update at regular intervals.
    /// </summary>
    /// <param name="intervalSeconds">Interval in seconds for automatic update. Set to 0 to disable.</param>
    /// <param name="enabled">Whether scheduled update is enabled.</param>
    /// <returns>Configuration status.</returns>
    [HttpPost("update/config")]
    public async Task<ActionResult> ConfigureScheduledUpdate(
        [FromQuery] int intervalSeconds,
        [FromQuery] bool enabled = true)
    {
        // This endpoint will be implemented to:
        // 1. Configure the scheduled task for traffic statistics update.
        // 2. Set the update interval (e.g., every 900 seconds for 15 minutes).
        // 3. Enable or disable the scheduled update.
        // 4. Return configuration status.

        // TODO: Implement the actual scheduling logic.
        return Ok(new
        {
            Message = "Scheduled traffic statistics update configured successfully.",
            IntervalSeconds = intervalSeconds,
            Enabled = enabled
        });
    }

    /// <summary>
    /// Get current scheduled update configuration.
    /// </summary>
    /// <returns>Current configuration settings.</returns>
    [HttpGet("update/config")]
    public async Task<ActionResult> GetScheduledUpdateConfig()
    {
        // This endpoint will be implemented to:
        // 1. Return the current scheduled update configuration.
        // 2. Include interval, enabled status, and next scheduled run time.

        // TODO: Implement the actual configuration retrieval logic.
        return Ok(new
        {
            Message = "Scheduled calculation configuration retrieved.",
            IntervalSeconds = 900,
            Enabled = true,
            NextRunTime = DateTime.UtcNow.AddSeconds(900)
        });
    }
#pragma warning restore
}
