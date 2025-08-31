using DbApp.Application.ResourceSystem.InspectionRecords;  
using MediatR;  
using Microsoft.AspNetCore.Mvc;  
  
namespace DbApp.Presentation.Controllers.ResourceSystem;  
  
[ApiController]  
[Route("api/resource/inspection-records")]  
public class InspectionRecordsController(IMediator mediator) : ControllerBase  
{  
    private readonly IMediator _mediator = mediator;  
  
    /// <summary>  
    /// Search inspection records by ride ID with filtering options.  
    /// </summary>  
    /// <param name="rideId">Ride ID.</param>  
    /// <param name="query">Search parameters.</param>  
    /// <returns>Paginated inspection record results.</returns>  
    [HttpGet("ride/{rideId}/search")]  
    public async Task<ActionResult<InspectionRecordResult>> SearchByRide(  
        [FromRoute] int rideId,  
        [FromQuery] SearchInspectionRecordsByRideQuery query)  
    {  
        var queryWithRideId = query with { RideId = rideId };  
        var result = await _mediator.Send(queryWithRideId);  
        return Ok(result);  
    }  
  
    /// <summary>  
    /// Search inspection records by multiple criteria (admin use).  
    /// </summary>  
    /// <param name="query">Search parameters.</param>  
    /// <returns>Paginated inspection record results.</returns>  
    [HttpGet("search")]  
    public async Task<ActionResult<InspectionRecordResult>> Search(  
        [FromQuery] SearchInspectionRecordsQuery query)  
    {  
        var result = await _mediator.Send(query);  
        return Ok(result);  
    }  
  
    /// <summary>  
    /// Get inspection record statistics.  
    /// </summary>  
    /// <param name="query">Statistics parameters.</param>  
    /// <returns>Inspection record statistics.</returns>  
    [HttpGet("stats/search")]  
    public async Task<ActionResult<InspectionRecordStatsDto>> GetStats(  
        [FromQuery] GetInspectionRecordStatsQuery query)  
    {  
        var result = await _mediator.Send(query);  
        return Ok(result);  
    }  
  
    /// <summary>  
    /// Get inspection record by ID.  
    /// </summary>  
    /// <param name="id">Inspection ID.</param>  
    /// <returns>Inspection record details.</returns>  
    [HttpGet("{id}")]  
    public async Task<ActionResult<InspectionRecordSummaryDto>> GetById(int id)  
    {  
        var record = await _mediator.Send(new GetInspectionRecordByIdQuery(id));  
        return record == null ? NotFound() : Ok(record);  
    }  
}