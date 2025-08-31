using DbApp.Application.ResourceSystem.MaintenanceRecords;  
using MediatR;  
using Microsoft.AspNetCore.Mvc;  
  
namespace DbApp.Presentation.Controllers.ResourceSystem;  
  
[ApiController]  
[Route("api/resource/maintenance-records")]  
public class MaintenanceRecordsController(IMediator mediator) : ControllerBase  
{  
    private readonly IMediator _mediator = mediator;  
  
    /// <summary>  
    /// Search maintenance records by ride ID with filtering options.  
    /// </summary>  
    /// <param name="rideId">Ride ID.</param>  
    /// <param name="query">Search parameters.</param>  
    /// <returns>Paginated maintenance record results.</returns>  
    [HttpGet("ride/{rideId}/search")]  
    public async Task<ActionResult<MaintenanceRecordResult>> SearchByRide(  
        [FromRoute] int rideId,  
        [FromQuery] SearchMaintenanceRecordsByRideQuery query)  
    {  
        var queryWithRideId = query with { RideId = rideId };  
        var result = await _mediator.Send(queryWithRideId);  
        return Ok(result);  
    }  
  
    /// <summary>  
    /// Search maintenance records by completion status.  
    /// </summary>  
    /// <param name="isCompleted">Completion status.</param>  
    /// <param name="query">Search parameters.</param>  
    /// <returns>Paginated maintenance record results.</returns>  
    [HttpGet("status/{isCompleted}/search")]  
    public async Task<ActionResult<MaintenanceRecordResult>> SearchByStatus(  
        [FromRoute] bool isCompleted,  
        [FromQuery] SearchMaintenanceRecordsByStatusQuery query)  
    {  
        var queryWithStatus = query with { IsCompleted = isCompleted };  
        var result = await _mediator.Send(queryWithStatus);  
        return Ok(result);  
    }  
  
    /// <summary>  
    /// Search maintenance records by multiple criteria (admin use).  
    /// </summary>  
    /// <param name="query">Search parameters.</param>  
    /// <returns>Paginated maintenance record results.</returns>  
    [HttpGet("search")]  
    public async Task<ActionResult<MaintenanceRecordResult>> Search(  
        [FromQuery] SearchMaintenanceRecordsQuery query)  
    {  
        var result = await _mediator.Send(query);  
        return Ok(result);  
    }  
  
    /// <summary>  
    /// Get maintenance record statistics.  
    /// </summary>  
    /// <param name="query">Statistics parameters.</param>  
    /// <returns>Maintenance record statistics.</returns>  
    [HttpGet("stats/search")]  
    public async Task<ActionResult<MaintenanceRecordStatsDto>> GetStats(  
        [FromQuery] GetMaintenanceRecordStatsQuery query)  
    {  
        var result = await _mediator.Send(query);  
        return Ok(result);  
    }  
  
    /// <summary>  
    /// Get maintenance record by ID.  
    /// </summary>  
    /// <param name="id">Maintenance ID.</param>  
    /// <returns>Maintenance record details.</returns>  
    [HttpGet("{id}")]  
    public async Task<ActionResult<MaintenanceRecordSummaryDto>> GetById(int id)  
    {  
        var record = await _mediator.Send(new GetMaintenanceRecordByIdQuery(id));  
        return record == null ? NotFound() : Ok(record);  
    }  
}