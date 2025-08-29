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
}