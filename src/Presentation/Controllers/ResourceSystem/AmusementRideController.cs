using DbApp.Application.ResourceSystem.AmusementRides;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/rides")]
public class AmusementRidesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Create a new amusement ride.
    /// </summary>
    /// <param name="command">Create command.</param>
    /// <returns>Created ride ID.</returns>
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateAmusementRideCommand command)
    {
        var rideId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = rideId }, rideId);
    }

    /// <summary>
    /// Get amusement ride by ID.
    /// </summary>
    /// <param name="id">Ride ID.</param>
    /// <returns>Amusement ride details.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<AmusementRideSummaryDto>> GetById(int id)
    {
        var ride = await _mediator.Send(new GetAmusementRideByIdQuery(id));
        return ride == null ? NotFound() : Ok(ride);
    }

    /// <summary>
    /// Update an existing amusement ride.
    /// </summary>
    /// <param name="id">Ride ID.</param>
    /// <param name="command">Update command.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateAmusementRideCommand command)
    {
        if (id != command.RideId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete an amusement ride.
    /// </summary>
    /// <param name="id">Ride ID.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteAmusementRideCommand(id));
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// Search amusement rides with comprehensive filtering options.
    /// </summary>
    /// <param name="keyword">Keyword for ride name or description.</param>
    /// <param name="status">Ride status filter.</param>
    /// <param name="location">Location filter.</param>
    /// <param name="managerId">Manager ID filter.</param>
    /// <param name="minCapacity">Minimum capacity filter.</param>
    /// <param name="maxCapacity">Maximum capacity filter.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paginated amusement ride results.</returns>
    [HttpGet("search")]
    public async Task<ActionResult<AmusementRideResult>> Search(
        [FromQuery] string? keyword = null,
        [FromQuery] RideStatus? status = null,
        [FromQuery] string? location = null,
        [FromQuery] int? managerId = null,
        [FromQuery] int? minCapacity = null,
        [FromQuery] int? maxCapacity = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new SearchAmusementRidesQuery(
            keyword, status, location, managerId, minCapacity, maxCapacity, page, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Get amusement ride statistics.
    /// </summary>
    /// <param name="startDate">Start date for statistics.</param>
    /// <param name="endDate">End date for statistics.</param>
    /// <returns>Amusement ride statistics.</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<AmusementRideStatsDto>> GetStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var result = await _mediator.Send(new GetAmusementRideStatsQuery(startDate, endDate));
        return Ok(result);
    }
}
