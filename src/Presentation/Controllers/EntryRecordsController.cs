using DbApp.Application.UserSystem.EntryRecords;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers;

/// <summary>
/// API controller for visitor entry/exit management.
/// Provides endpoints for registering entries, exits, and querying visitor statistics.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EntryRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Register a visitor's entry into the park.
    /// </summary>
    /// <param name="command">Entry registration command.</param>
    /// <returns>The ID of the created entry record.</returns>
    [HttpPost("entry")]
    public async Task<IActionResult> RegisterEntry(RegisterEntryCommand command)
    {
        try
        {
            var entryRecordId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = entryRecordId }, new { EntryRecordId = entryRecordId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Register a visitor's exit from the park.
    /// </summary>
    /// <param name="command">Exit registration command.</param>
    /// <returns>Success response.</returns>
    [HttpPost("exit")]
    public async Task<IActionResult> RegisterExit(RegisterExitCommand command)
    {
        try
        {
            await _mediator.Send(command);
            return Ok(new { Message = "Exit registered successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Get all entry records.
    /// </summary>
    /// <returns>List of all entry records.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var entryRecords = await _mediator.Send(new GetAllEntryRecordsQuery());
        return Ok(entryRecords);
    }

    /// <summary>
    /// Get an entry record by ID.
    /// </summary>
    /// <param name="id">The entry record ID.</param>
    /// <returns>The entry record if found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entryRecord = await _mediator.Send(new GetEntryRecordByIdQuery(id));
        if (entryRecord == null)
        {
            return NotFound(new { Error = $"Entry record with ID {id} not found" });
        }
        return Ok(entryRecord);
    }

    /// <summary>
    /// Get entry records for a specific visitor.
    /// </summary>
    /// <param name="visitorId">The visitor ID.</param>
    /// <returns>List of entry records for the visitor.</returns>
    [HttpGet("visitor/{visitorId}")]
    public async Task<IActionResult> GetByVisitorId(int visitorId)
    {
        var entryRecords = await _mediator.Send(new GetEntryRecordsByVisitorIdQuery(visitorId));
        return Ok(entryRecords);
    }

    /// <summary>
    /// Get current visitors in the park.
    /// </summary>
    /// <returns>List of visitors currently in the park.</returns>
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentVisitors()
    {
        var currentVisitors = await _mediator.Send(new GetCurrentVisitorsQuery());
        return Ok(currentVisitors);
    }

    /// <summary>
    /// Get current visitor count in the park.
    /// </summary>
    /// <returns>Number of visitors currently in the park.</returns>
    [HttpGet("current/count")]
    public async Task<IActionResult> GetCurrentVisitorCount()
    {
        var count = await _mediator.Send(new GetCurrentVisitorCountQuery());
        return Ok(new { CurrentVisitorCount = count });
    }

    /// <summary>
    /// Get entry records within a date range.
    /// </summary>
    /// <param name="startDate">Start date (YYYY-MM-DD format).</param>
    /// <param name="endDate">End date (YYYY-MM-DD format).</param>
    /// <returns>List of entry records within the date range.</returns>
    [HttpGet("date-range")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest(new { Error = "Start date cannot be later than end date" });
        }

        var entryRecords = await _mediator.Send(new GetEntryRecordsByDateRangeQuery(startDate, endDate));
        return Ok(entryRecords);
    }

    /// <summary>
    /// Get daily visitor statistics.
    /// </summary>
    /// <param name="date">The date to get statistics for (YYYY-MM-DD format).</param>
    /// <returns>Daily statistics including entries, exits, and current count.</returns>
    [HttpGet("statistics/daily")]
    public async Task<IActionResult> GetDailyStatistics([FromQuery] DateTime date)
    {
        var statistics = await _mediator.Send(new GetDailyStatisticsQuery(date));
        return Ok(statistics);
    }

    /// <summary>
    /// Get entry records by entry gate.
    /// </summary>
    /// <param name="gate">The entry gate name.</param>
    /// <returns>List of entry records for the specified gate.</returns>
    [HttpGet("gate/{gate}")]
    public async Task<IActionResult> GetByGate(string gate)
    {
        var entryRecords = await _mediator.Send(new GetEntryRecordsByGateQuery(gate));
        return Ok(entryRecords);
    }

    /// <summary>
    /// Get active entry for a visitor (if still in park).
    /// </summary>
    /// <param name="visitorId">The visitor ID.</param>
    /// <returns>The active entry record if visitor is in park.</returns>
    [HttpGet("visitor/{visitorId}/active")]
    public async Task<IActionResult> GetActiveEntryForVisitor(int visitorId)
    {
        var activeEntry = await _mediator.Send(new GetActiveEntryForVisitorQuery(visitorId));
        if (activeEntry == null)
        {
            return NotFound(new { Error = $"No active entry found for visitor {visitorId}" });
        }
        return Ok(activeEntry);
    }

    /// <summary>
    /// Update an entry record.
    /// </summary>
    /// <param name="id">The entry record ID.</param>
    /// <param name="command">Update command.</param>
    /// <returns>Success response.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateEntryRecordCommand command)
    {
        if (id != command.EntryRecordId)
        {
            return BadRequest(new { Error = "ID in URL does not match ID in request body" });
        }

        try
        {
            await _mediator.Send(command);
            return Ok(new { Message = "Entry record updated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Delete an entry record.
    /// </summary>
    /// <param name="id">The entry record ID.</param>
    /// <returns>Success response.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _mediator.Send(new DeleteEntryRecordCommand(id));
            return Ok(new { Message = "Entry record deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }
}
