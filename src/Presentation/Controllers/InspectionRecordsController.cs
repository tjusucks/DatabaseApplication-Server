using DbApp.Application.ResourceSystem.InspectionRecords;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/inspection-records")]
public class InspectionRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("ride/{rideId}/search")]
    public async Task<ActionResult<InspectionRecordResult>> SearchByRide(
        [FromRoute] int rideId,
        [FromQuery] SearchInspectionRecordsByRideQuery query)
    {
        var queryWithRideId = query with { RideId = rideId };
        var result = await _mediator.Send(queryWithRideId);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<InspectionRecordResult>> Search(
        [FromQuery] SearchInspectionRecordsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("stats/search")]
    public async Task<ActionResult<InspectionRecordStatsDto>> GetStats(
        [FromQuery] GetInspectionRecordStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InspectionRecordSummaryDto>> GetById(int id)
    {
        var record = await _mediator.Send(new GetInspectionRecordByIdQuery(id));
        return record == null ? NotFound() : Ok(record);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateInspectionRecordCommand command)
    {
        var recordId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = recordId }, recordId);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateInspectionRecordCommand command)
    {
        if (id != command.InspectionId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteInspectionRecordCommand(id));
        return result ? NoContent() : NotFound();
    }
}
