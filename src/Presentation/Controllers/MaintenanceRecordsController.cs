using DbApp.Application.ResourceSystem.MaintenanceRecords;
using DbApp.Domain.Entities.ResourceSystem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<int>> CreateMaintenanceRecord([FromBody] CreateMaintenanceRecordCommand command)
    {
        var recordId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMaintenanceRecord), new { id = recordId }, recordId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MaintenanceRecord>> GetMaintenanceRecord(int id)
    {
        var record = await _mediator.Send(new GetMaintenanceRecordByIdQuery(id));
        return record == null ? NotFound() : Ok(record);
    }

    [HttpGet]
    public async Task<ActionResult<List<MaintenanceRecord>>> GetAllMaintenanceRecords()
    {
        var records = await _mediator.Send(new GetAllMaintenanceRecordsQuery());
        return Ok(records);
    }

    [HttpGet("ride/{rideId}")]
    public async Task<ActionResult<List<MaintenanceRecord>>> GetMaintenanceRecordsByRide(int rideId)
    {
        var records = await _mediator.Send(new GetMaintenanceRecordsByRideQuery(rideId));
        return Ok(records);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<List<MaintenanceRecord>>> GetPendingMaintenanceRecords()
    {
        var records = await _mediator.Send(new GetPendingMaintenanceRecordsQuery());
        return Ok(records);
    }

    [HttpGet("team/{teamId}")]
    public async Task<ActionResult<List<MaintenanceRecord>>> GetMaintenanceRecordsByTeam(int teamId)
    {
        var records = await _mediator.Send(new GetMaintenanceRecordsByTeamQuery(teamId));
        return Ok(records);
    }

    [HttpGet("daterange")]
    public async Task<ActionResult<List<MaintenanceRecord>>> GetMaintenanceRecordsByDateRange(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var records = await _mediator.Send(new GetMaintenanceRecordsByDateRangeQuery(startDate, endDate));
        return Ok(records);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateMaintenanceRecord(int id, [FromBody] UpdateMaintenanceRecordCommand command)
    {
        if (id != command.MaintenanceId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id}/complete")]
    public async Task<ActionResult> CompleteMaintenanceRecord(int id, [FromBody] CompleteMaintenanceCommand command)
    {
        if (id != command.MaintenanceId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id}/accept")]
    public async Task<ActionResult> AcceptMaintenanceRecord(int id, [FromBody] AcceptMaintenanceCommand command)
    {
        if (id != command.MaintenanceId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMaintenanceRecord(int id)
    {
        await _mediator.Send(new DeleteMaintenanceRecordCommand(id));
        return NoContent();
    }
}
