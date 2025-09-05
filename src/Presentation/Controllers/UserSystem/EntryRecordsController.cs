using DbApp.Application.UserSystem.EntryRecords;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.UserSystem;

/// <summary>
/// API controller for entry record management.
/// Provides endpoints for visitor entry/exit tracking and statistics.
/// </summary>
[ApiController]
[Route("api/user/entries")]
public class EntryRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Create a new entry or exit record based on type.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEntryRecordCommand command)
    {
        try
        {
            var entryRecordId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = entryRecordId }, new { EntryRecordId = entryRecordId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Get all entry records.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllEntryRecordsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get an entry record by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var entryRecord = await _mediator.Send(new GetEntryRecordByIdQuery(id));
        if (entryRecord == null)
            return NotFound(new { Error = $"Entry record with ID {id} not found" });
        return Ok(entryRecord);
    }

    /// <summary>
    /// Update an entry record.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateEntryRecordCommand command)
    {
        if (id != command.EntryRecordId)
            return BadRequest(new { Error = "ID in URL does not match ID in request body" });

        await _mediator.Send(command);
        return Ok(new { Message = "Entry record updated successfully" });
    }

    /// <summary>
    /// Delete an entry record.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _mediator.Send(new DeleteEntryRecordCommand(id));
        return Ok(new { Message = "Entry record deleted successfully" });
    }

    /// <summary>
    /// Search entry records with filtering, pagination and sorting.
    /// Supports current status, filtering, grouping, and pagination queries.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchEntryRecordsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get entry record statistics with filtering.
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] GetEntryRecordStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get grouped entry record statistics with filtering.
    /// </summary>
    [HttpGet("stats/grouped")]
    public async Task<IActionResult> GetGroupedStats([FromQuery] GetGroupedEntryRecordStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
