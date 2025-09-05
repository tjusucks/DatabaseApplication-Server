using DbApp.Application.UserSystem.Visitors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.UserSystem;

/// <summary>
/// API controller for visitor management.
/// Provides endpoints for visitor information recording, updating, and querying historical data.
/// </summary>
[ApiController]
[Route("api/user/visitors")]
public class VisitorsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Create a new visitor with complete user information.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVisitorCommand command)
    {
        var visitorId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = visitorId }, new { VisitorId = visitorId });
    }

    /// <summary>
    /// Get all visitors.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllVisitorsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a visitor by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var visitor = await _mediator.Send(new GetVisitorByIdQuery(id));
        if (visitor == null)
            return NotFound(new { Error = $"Visitor with ID {id} not found" });
        return Ok(visitor);
    }

    /// <summary>
    /// Update visitor information.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateVisitorCommand command)
    {
        if (id != command.VisitorId)
            return BadRequest(new { Error = "ID in URL does not match ID in request body" });

        await _mediator.Send(command);
        return Ok(new { Message = "Visitor updated successfully" });
    }

    /// <summary>
    /// Delete a visitor.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _mediator.Send(new DeleteVisitorCommand(id));
        return Ok(new { Message = "Visitor deleted successfully" });
    }

    /// <summary>
    /// RESTful unified search API for visitors.
    /// Supports keyword search, filtering, and pagination.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchVisitorsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get visitor statistics with filtering.
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] GetVisitorStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get grouped visitor statistics with filtering.
    /// </summary>
    [HttpGet("stats/grouped")]
    public async Task<IActionResult> GetGroupedStats([FromQuery] GetGroupedVisitorStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }


    /// <summary>
    /// Blacklist a visitor.
    /// </summary>
    [HttpPost("{id}/blacklist")]
    public async Task<IActionResult> Blacklist([FromRoute] int id, [FromBody] BlacklistVisitorCommand command)
    {
        if (id != command.VisitorId)
            return BadRequest(new { Error = "ID in URL does not match ID in request body" });

        await _mediator.Send(command);
        return Ok(new { Message = "Visitor blacklisted successfully" });
    }

    /// <summary>
    /// Remove a visitor from blacklist.
    /// </summary>
    [HttpDelete("{id}/blacklist")]
    public async Task<IActionResult> Unblacklist([FromRoute] int id)
    {
        await _mediator.Send(new UnblacklistVisitorCommand(id));
        return Ok(new { Message = "Visitor removed from blacklist successfully" });
    }

    /// <summary>
    /// Upgrade a visitor to member status.
    /// </summary>
    [HttpPost("{id}/membership")]
    public async Task<IActionResult> UpgradeToMember([FromRoute] int id)
    {
        await _mediator.Send(new UpgradeToMemberCommand(id));
        return Ok(new { Message = "Visitor upgraded to member successfully" });
    }

    /// <summary>
    /// Remove membership from a visitor.
    /// </summary>
    [HttpDelete("{id}/membership")]
    public async Task<IActionResult> RemoveMembership([FromRoute] int id)
    {
        await _mediator.Send(new RemoveMembershipCommand(id));
        return Ok(new { Message = "Visitor membership removed successfully" });
    }

    /// <summary>
    /// Add points to a visitor.
    /// </summary>
    [HttpPost("{id}/points/add")]
    public async Task<IActionResult> AddPoints([FromRoute] int id, [FromBody] AddPointsCommand command)
    {
        if (id != command.VisitorId)
            return BadRequest(new { Error = "ID in URL does not match ID in request body" });

        await _mediator.Send(command);
        return Ok(new { Message = "Points added successfully" });
    }

    /// <summary>
    /// Deduct points from a visitor.
    /// </summary>
    [HttpPost("{id}/points/deduct")]
    public async Task<IActionResult> DeductPoints([FromRoute] int id, [FromBody] DeductPointsCommand command)
    {
        if (id != command.VisitorId)
            return BadRequest(new { Error = "ID in URL does not match ID in request body" });

        await _mediator.Send(command);
        return Ok(new { Message = "Points deducted successfully" });
    }
}
