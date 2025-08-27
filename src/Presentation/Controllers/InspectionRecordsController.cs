using DbApp.Application.ResourceSystem.InspectionRecords;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InspectionRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<int>> CreateInspectionRecord([FromBody] CreateInspectionRecordCommand command)
    {
        var recordId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetInspectionRecord), new { id = recordId }, recordId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InspectionRecord>> GetInspectionRecord(int id)
    {
        var record = await _mediator.Send(new GetInspectionRecordByIdQuery(id));
        return record == null ? NotFound() : Ok(record);
    }

    [HttpGet]
    public async Task<ActionResult<List<InspectionRecord>>> GetAllInspectionRecords()
    {
        var records = await _mediator.Send(new GetAllInspectionRecordsQuery());
        return Ok(records);
    }

    [HttpGet("ride/{rideId}")]
    public async Task<ActionResult<List<InspectionRecord>>> GetInspectionRecordsByRide(int rideId)
    {
        var records = await _mediator.Send(new GetInspectionRecordsByRideQuery(rideId));
        return Ok(records);
    }

    [HttpGet("failed")]
    public async Task<ActionResult<List<InspectionRecord>>> GetFailedInspections()
    {
        var records = await _mediator.Send(new GetFailedInspectionsQuery());
        return Ok(records);
    }

    [HttpGet("type/{checkType}")]
    public async Task<ActionResult<List<InspectionRecord>>> GetInspectionRecordsByType(CheckType checkType)
    {
        var records = await _mediator.Send(new GetInspectionRecordsByTypeQuery(checkType));
        return Ok(records);
    }

    [HttpGet("daterange")]
    public async Task<ActionResult<List<InspectionRecord>>> GetInspectionRecordsByDateRange(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var records = await _mediator.Send(new GetInspectionRecordsByDateRangeQuery(startDate, endDate));
        return Ok(records);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateInspectionRecord(int id, [FromBody] UpdateInspectionRecordCommand command)
    {
        if (id != command.InspectionId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id}/complete")]
    public async Task<ActionResult> CompleteInspectionRecord(int id, [FromBody] CompleteInspectionCommand command)
    {
        if (id != command.InspectionId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteInspectionRecord(int id)
    {
        await _mediator.Send(new DeleteInspectionRecordCommand(id));
        return NoContent();
    }
}
