using DbApp.Application.ResourceSystem.InspectionRecords;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/inspections")]  // 简化路径  
public class InspectionRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>    
    /// Search inspection records with comprehensive filtering options.    
    /// </summary>    
    /// <param name="searchTerm">Search term for issues or recommendations.</param>    
    /// <param name="rideId">Ride ID filter.</param>  
    /// <param name="teamId">Team ID filter.</param>  
    /// <param name="checkType">Check type filter.</param>  
    /// <param name="isPassed">Pass status filter.</param>  
    /// <param name="checkDateFrom">Check date from filter.</param>  
    /// <param name="checkDateTo">Check date to filter.</param>  
    /// <param name="page">Page number.</param>    
    /// <param name="pageSize">Page size.</param>    
    /// <returns>Paginated inspection record results.</returns>    
    [HttpGet("search")]
    public async Task<ActionResult<InspectionRecordResult>> Search(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? rideId = null,
        [FromQuery] int? teamId = null,
        [FromQuery] CheckType? checkType = null,
        [FromQuery] bool? isPassed = null,
        [FromQuery] DateTime? checkDateFrom = null,
        [FromQuery] DateTime? checkDateTo = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new SearchInspectionRecordsQuery(
            searchTerm, rideId, teamId, checkType, isPassed, checkDateFrom, checkDateTo, page, pageSize));
        return Ok(result);
    }

    /// <summary>    
    /// Get inspection record statistics.    
    /// </summary>    
    /// <param name="startDate">Start date for statistics.</param>    
    /// <param name="endDate">End date for statistics.</param>    
    /// <param name="rideId">Ride ID filter for stats.</param>  
    /// <returns>Inspection record statistics.</returns>    
    [HttpGet("stats")]
    public async Task<ActionResult<InspectionRecordStatsDto>> GetStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? rideId = null)
    {
        var result = await _mediator.Send(new GetInspectionRecordStatsQuery(startDate, endDate, rideId));
        return Ok(result);
    }

    /// <summary>    
    /// Get inspection record by ID.    
    /// </summary>    
    /// <param name="id">Inspection record ID.</param>    
    /// <returns>Inspection record details.</returns>    
    [HttpGet("{id}")]
    public async Task<ActionResult<InspectionRecordSummaryDto>> GetById(int id)
    {
        var record = await _mediator.Send(new GetInspectionRecordByIdQuery(id));
        return record == null ? NotFound() : Ok(record);
    }

    /// <summary>    
    /// Create a new inspection record.    
    /// </summary>    
    /// <param name="command">Create command.</param>    
    /// <returns>Created inspection record ID.</returns>    
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateInspectionRecordCommand command)
    {
        var recordId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = recordId }, recordId);
    }

    /// <summary>    
    /// Update an existing inspection record.    
    /// </summary>    
    /// <param name="id">Inspection record ID.</param>    
    /// <param name="command">Update command.</param>    
    /// <returns>No content on success.</returns>    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateInspectionRecordCommand command)
    {
        if (id != command.InspectionId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>    
    /// Delete an inspection record.    
    /// </summary>    
    /// <param name="id">Inspection record ID.</param>    
    /// <returns>No content on success.</returns>    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteInspectionRecordCommand(id));
        return result ? NoContent() : NotFound();
    }
}
