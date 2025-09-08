using DbApp.Application.ResourceSystem.MaintenanceRecords;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/maintenance")]
public class MaintenanceRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Create a new maintenance record.
    /// </summary>
    /// <param name="command">Create command.</param>
    /// <returns>Created maintenance record ID.</returns>
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateMaintenanceRecordCommand command)
    {
        var recordId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = recordId }, recordId);
    }

    /// <summary>
    /// Get maintenance record by ID.
    /// </summary>
    /// <param name="id">Maintenance record ID.</param>
    /// <returns>Maintenance record details.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<MaintenanceRecordSummaryDto>> GetById(int id)
    {
        var record = await _mediator.Send(new GetMaintenanceRecordByIdQuery(id));
        return record == null ? NotFound() : Ok(record);
    }

    /// <summary>
    /// Update an existing maintenance record.
    /// </summary>
    /// <param name="id">Maintenance record ID.</param>
    /// <param name="command">Update command.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateMaintenanceRecordCommand command)
    {
        if (id != command.MaintenanceId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete a maintenance record.
    /// </summary>
    /// <param name="id">Maintenance record ID.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteMaintenanceRecordCommand(id));
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// Search maintenance records with comprehensive filtering options.
    /// </summary>
    /// <param name="keyword">Keyword for maintenance details or parts replaced.</param>
    /// <param name="rideId">Ride ID filter.</param>
    /// <param name="teamId">Team ID filter.</param>
    /// <param name="managerId">Manager ID filter.</param>
    /// <param name="maintenanceType">Maintenance type filter.</param>
    /// <param name="isCompleted">Completion status filter.</param>
    /// <param name="isAccepted">Acceptance status filter.</param>
    /// <param name="startTimeFrom">Start time from filter.</param>
    /// <param name="startTimeTo">Start time to filter.</param>
    /// <param name="minCost">Minimum cost filter.</param>
    /// <param name="maxCost">Maximum cost filter.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paginated maintenance record results.</returns>
    [HttpGet("search")]
    public async Task<ActionResult<MaintenanceRecordResult>> Search(
        [FromQuery] string? keyword = null,
        [FromQuery] int? rideId = null,
        [FromQuery] int? teamId = null,
        [FromQuery] int? managerId = null,
        [FromQuery] MaintenanceType? maintenanceType = null,
        [FromQuery] bool? isCompleted = null,
        [FromQuery] bool? isAccepted = null,
        [FromQuery] DateTime? startTimeFrom = null,
        [FromQuery] DateTime? startTimeTo = null,
        [FromQuery] decimal? minCost = null,
        [FromQuery] decimal? maxCost = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new SearchMaintenanceRecordsQuery(
            keyword, rideId, teamId, managerId, maintenanceType, isCompleted, isAccepted,
            startTimeFrom, startTimeTo, null, null, minCost, maxCost, page, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Get maintenance record statistics.
    /// </summary>
    /// <param name="startDate">Start date for statistics.</param>
    /// <param name="endDate">End date for statistics.</param>
    /// <param name="rideId">Ride ID filter for stats.</param>
    /// <returns>Maintenance record statistics.</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<MaintenanceRecordStatsDto>> GetStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? rideId = null)
    {
        var result = await _mediator.Send(new GetMaintenanceRecordStatsQuery(startDate, endDate, rideId));
        return Ok(result);
    }
}
