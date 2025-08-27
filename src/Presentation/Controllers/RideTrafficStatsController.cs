using DbApp.Application.ResourceSystem.RideTrafficStats;
using DbApp.Domain.Entities.ResourceSystem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RideTrafficStatsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult> CreateRideTrafficStat([FromBody] CreateRideTrafficStatCommand command)
    {
        await _mediator.Send(command);
        return CreatedAtAction(nameof(GetRideTrafficStat),
            new { rideId = command.RideId, recordTime = command.RecordTime }, null);
    }

    [HttpGet("{rideId}/{recordTime}")]
    public async Task<ActionResult<RideTrafficStat>> GetRideTrafficStat(int rideId, DateTime recordTime)
    {
        var stat = await _mediator.Send(new GetRideTrafficStatByIdQuery(rideId, recordTime));
        return stat == null ? NotFound() : Ok(stat);
    }

    [HttpGet("ride/{rideId}")]
    public async Task<ActionResult<List<RideTrafficStat>>> GetRideTrafficStatsByRide(int rideId)
    {
        var stats = await _mediator.Send(new GetRideTrafficStatsByRideQuery(rideId));
        return Ok(stats);
    }

    [HttpGet("ride/{rideId}/daterange")]
    public async Task<ActionResult<List<RideTrafficStat>>> GetRideTrafficStatsByDateRange(
        int rideId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var stats = await _mediator.Send(new GetRideTrafficStatsByDateRangeQuery(rideId, startDate, endDate));
        return Ok(stats);
    }

    [HttpGet("crowded")]
    public async Task<ActionResult<List<RideTrafficStat>>> GetCrowdedRides([FromQuery] DateTime? date = null)
    {
        var stats = await _mediator.Send(new GetCrowdedRidesQuery(date));
        return Ok(stats);
    }

    [HttpGet("popularity")]
    public async Task<ActionResult<List<RideTrafficStat>>> GetRidePopularityReport(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var stats = await _mediator.Send(new GetRidePopularityReportQuery(startDate, endDate));
        return Ok(stats);
    }

    [HttpGet("ride/{rideId}/peakhours")]
    public async Task<ActionResult<List<RideTrafficStat>>> GetPeakHoursAnalysis(
        int rideId, [FromQuery] DateTime date)
    {
        var stats = await _mediator.Send(new GetPeakHoursAnalysisQuery(rideId, date));
        return Ok(stats);
    }

    [HttpPut("{rideId}/{recordTime}")]
    public async Task<ActionResult> UpdateRideTrafficStat(
        int rideId, DateTime recordTime, [FromBody] UpdateRideTrafficStatCommand command)
    {
        if (rideId != command.RideId || recordTime != command.RecordTime)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{rideId}/{recordTime}")]
    public async Task<ActionResult> DeleteRideTrafficStat(int rideId, DateTime recordTime)
    {
        await _mediator.Send(new DeleteRideTrafficStatCommand(rideId, recordTime));
        return NoContent();
    }
}
