using DbApp.Application.UserSystem.Visitors;
using DbApp.Domain.Enums.UserSystem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers;

/// <summary>
/// API controller for visitor management.
/// Provides endpoints for visitor information recording, updating, and querying historical data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VisitorsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Create a new visitor with complete user information.
    /// </summary>
    /// <param name="command">Visitor creation command.</param>
    /// <returns>The ID of the created visitor.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(CreateVisitorCommand command)
    {
        try
        {
            var visitorId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = visitorId }, new { VisitorId = visitorId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Get all visitors.
    /// </summary>
    /// <returns>List of all visitors.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var visitors = await _mediator.Send(new GetAllVisitorsQuery());
        return Ok(visitors);
    }

    /// <summary>
    /// Get a visitor by ID.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <returns>The visitor information.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var visitor = await _mediator.Send(new GetVisitorByIdQuery(id));
        if (visitor == null)
        {
            return NotFound(new { Error = $"Visitor with ID {id} not found" });
        }
        return Ok(visitor);
    }

    /// <summary>
    /// Get visitor by user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The visitor information.</returns>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var visitor = await _mediator.Send(new GetVisitorByUserIdQuery(userId));
        if (visitor == null)
        {
            return NotFound(new { Error = $"Visitor with user ID {userId} not found" });
        }
        return Ok(visitor);
    }

    /// <summary>
    /// Get visitor history information including entry records.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <returns>Complete visitor history information.</returns>
    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetHistory(int id)
    {
        var history = await _mediator.Send(new GetVisitorHistoryQuery(id));
        if (history == null)
        {
            return NotFound(new { Error = $"Visitor with ID {id} not found" });
        }
        return Ok(history);
    }

    /// <summary>
    /// Search visitors by name.
    /// </summary>
    /// <param name="name">The name to search for.</param>
    /// <returns>List of matching visitors.</returns>
    [HttpGet("search/name")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new { Error = "Name parameter is required" });
        }

        var visitors = await _mediator.Send(new SearchVisitorsByNameQuery(name));
        return Ok(visitors);
    }

    /// <summary>
    /// Search visitors by phone number.
    /// </summary>
    /// <param name="phoneNumber">The phone number to search for.</param>
    /// <returns>List of matching visitors.</returns>
    [HttpGet("search/phone")]
    public async Task<IActionResult> SearchByPhone([FromQuery] string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return BadRequest(new { Error = "Phone number parameter is required" });
        }

        var visitors = await _mediator.Send(new SearchVisitorsByPhoneQuery(phoneNumber));
        return Ok(visitors);
    }

    /// <summary>
    /// Get visitors by blacklist status.
    /// </summary>
    /// <param name="isBlacklisted">Whether to get blacklisted or non-blacklisted visitors.</param>
    /// <returns>List of visitors with the specified blacklist status.</returns>
    [HttpGet("blacklist/{isBlacklisted}")]
    public async Task<IActionResult> GetByBlacklistStatus(bool isBlacklisted)
    {
        var visitors = await _mediator.Send(new GetVisitorsByBlacklistStatusQuery(isBlacklisted));
        return Ok(visitors);
    }

    /// <summary>
    /// Get visitors by visitor type.
    /// </summary>
    /// <param name="visitorType">The visitor type to filter by.</param>
    /// <returns>List of visitors of the specified type.</returns>
    [HttpGet("type/{visitorType}")]
    public async Task<IActionResult> GetByType(VisitorType visitorType)
    {
        var visitors = await _mediator.Send(new GetVisitorsByTypeQuery(visitorType));
        return Ok(visitors);
    }

    /// <summary>
    /// Get visitors registered within a date range.
    /// </summary>
    /// <param name="startDate">Start date for registration.</param>
    /// <param name="endDate">End date for registration.</param>
    /// <returns>List of visitors registered within the date range.</returns>
    [HttpGet("registration-date")]
    public async Task<IActionResult> GetByRegistrationDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest(new { Error = "Start date must be before or equal to end date" });
        }

        var visitors = await _mediator.Send(new GetVisitorsByRegistrationDateRangeQuery(startDate, endDate));
        return Ok(visitors);
    }

    /// <summary>
    /// Search visitors with multiple criteria.
    /// </summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="phoneNumber">Optional phone number filter.</param>
    /// <param name="isBlacklisted">Optional blacklist status filter.</param>
    /// <param name="visitorType">Optional visitor type filter.</param>
    /// <param name="startDate">Optional start date for registration.</param>
    /// <param name="endDate">Optional end date for registration.</param>
    /// <returns>List of visitors matching the criteria.</returns>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name = null,
        [FromQuery] string? phoneNumber = null,
        [FromQuery] bool? isBlacklisted = null,
        [FromQuery] VisitorType? visitorType = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            return BadRequest(new { Error = "Start date must be before or equal to end date" });
        }

        var visitors = await _mediator.Send(new SearchVisitorsQuery(
            name, phoneNumber, isBlacklisted, visitorType, startDate, endDate));
        return Ok(visitors);
    }

    /// <summary>
    /// Update visitor information.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <param name="command">Update command.</param>
    /// <returns>Success response.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateVisitorCommand command)
    {
        if (id != command.VisitorId)
        {
            return BadRequest(new { Error = "ID in URL does not match ID in request body" });
        }

        try
        {
            await _mediator.Send(command);
            return Ok(new { Message = "Visitor updated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Update visitor blacklist status.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <param name="command">Blacklist status update command.</param>
    /// <returns>Success response.</returns>
    [HttpPut("{id}/blacklist")]
    public async Task<IActionResult> UpdateBlacklistStatus(int id, UpdateVisitorBlacklistStatusCommand command)
    {
        if (id != command.VisitorId)
        {
            return BadRequest(new { Error = "ID in URL does not match ID in request body" });
        }

        try
        {
            await _mediator.Send(command);
            return Ok(new { Message = "Visitor blacklist status updated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a visitor.
    /// </summary>
    /// <param name="id">The visitor ID.</param>
    /// <returns>Success response.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _mediator.Send(new DeleteVisitorCommand(id));
            return Ok(new { Message = "Visitor deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }
}
