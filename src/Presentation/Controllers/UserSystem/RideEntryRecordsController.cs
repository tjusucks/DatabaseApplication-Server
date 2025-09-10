using MediatR;
using Microsoft.AspNetCore.Mvc;
using DbApp.Application.UserSystem.RideEntryRecords;

namespace DbApp.Presentation.Controllers.UserSystem;

/// <summary>
/// API controller for ride entry record management.
/// Provides endpoints for visitor ride entry/exit tracking.
/// </summary>
[ApiController]
[Route("api/user/ride-entries")]
public class RideEntryRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Create a new ride entry or exit record.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRideEntryRecordCommand command)
    {
        var entryRecordId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = entryRecordId }, new { EntryRecordId = entryRecordId });
    }

    /// <summary>
    /// Get all ride entry records.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllRideEntryRecordsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Get a ride entry record by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var entryRecord = await _mediator.Send(new GetRideEntryRecordByIdQuery { EntryRecordId = id });
        return Ok(entryRecord);
    }

    /// <summary>
    /// Update a ride entry record.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateRideEntryRecordCommand command)
    {
        if (id != command.EntryRecordId)
            return BadRequest(new { Error = "ID in URL does not match ID in request body" });

        await _mediator.Send(command);
        return Ok(new { Message = "Ride entry record updated successfully" });
    }

    /// <summary>
    /// Delete a ride entry record.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _mediator.Send(new DeleteRideEntryRecordCommand(id));
        return Ok(new { Message = "Ride entry record deleted successfully" });
    }
}