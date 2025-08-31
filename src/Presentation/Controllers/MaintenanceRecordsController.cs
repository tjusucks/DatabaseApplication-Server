using DbApp.Application.ResourceSystem.MaintenanceRecords;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/maintenance-records")]
public class MaintenanceRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("ride/{rideId}/search")]
    public async Task<ActionResult<MaintenanceRecordResult>> SearchByRide(
        [FromRoute] int rideId,
        [FromQuery] SearchMaintenanceRecordsByRideQuery query)
    {
        var queryWithRideId = query with { RideId = rideId };
        var result = await _mediator.Send(queryWithRideId);
        return Ok(result);
    }

    [HttpGet("status/{isCompleted}/search")]
    public async Task<ActionResult<MaintenanceRecordResult>> SearchByStatus(
        [FromRoute] bool isCompleted,
        [FromQuery] SearchMaintenanceRecordsByStatusQuery query)
    {
        var queryWithStatus = query with { IsCompleted = isCompleted };
        var result = await _mediator.Send(queryWithStatus);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<MaintenanceRecordResult>> Search(
        [FromQuery] SearchMaintenanceRecordsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("stats/search")]
    public async Task<ActionResult<MaintenanceRecordStatsDto>> GetStats(
        [FromQuery] GetMaintenanceRecordStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MaintenanceRecordSummaryDto>> GetById(int id)
    {
        var record = await _mediator.Send(new GetMaintenanceRecordByIdQuery(id));
        return record == null ? NotFound() : Ok(record);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateMaintenanceRecordCommand command)
    {
        var recordId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = recordId }, recordId);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateMaintenanceRecordCommand command)
    {
        if (id != command.MaintenanceId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteMaintenanceRecordCommand(id));
        return result ? NoContent() : NotFound();
    }
}
