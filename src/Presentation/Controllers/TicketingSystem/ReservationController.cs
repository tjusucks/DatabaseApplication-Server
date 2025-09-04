using DbApp.Application.TicketingSystem.Reservations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/ticketing/reservations")]
public class ReservationController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Create a new reservation.
    /// </summary>
    /// <param name="command">Reservation creation parameters.</param>
    /// <returns>Created reservation details.</returns>
    [HttpPost]
    public async Task<ActionResult<CreateReservationResponseDto>> CreateReservation(
        [FromBody] CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetReservation), new { id = result.ReservationId }, result);
    }

    /// <summary>
    /// Get reservation by ID.
    /// </summary>
    /// <param name="id">Reservation ID.</param>
    /// <returns>Reservation details.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetReservation(int id)
    {
        var query = new GetReservationByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound($"Reservation {id} not found");
            
        return Ok(result);
    }

    /// <summary>
    /// Update reservation status.
    /// </summary>
    /// <param name="id">Reservation ID.</param>
    /// <param name="command">Status update parameters.</param>
    /// <returns>Updated reservation details.</returns>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ReservationDto>> UpdateReservationStatus(
        int id,
        [FromBody] UpdateReservationStatusCommand command)
    {
        var commandWithId = command with { ReservationId = id };
        var result = await _mediator.Send(commandWithId);
        return Ok(result);
    }

    /// <summary>
    /// Process payment for a reservation.
    /// </summary>
    /// <param name="id">Reservation ID.</param>
    /// <param name="command">Payment processing parameters.</param>
    /// <returns>Updated reservation details.</returns>
    [HttpPost("{id}/payment")]
    public async Task<ActionResult<ReservationDto>> ProcessPayment(
        int id,
        [FromBody] ProcessPaymentCommand command)
    {
        var commandWithId = command with { ReservationId = id };
        var result = await _mediator.Send(commandWithId);
        return Ok(result);
    }

    /// <summary>
    /// Cancel a reservation.
    /// </summary>
    /// <param name="id">Reservation ID.</param>
    /// <param name="command">Cancellation parameters.</param>
    /// <returns>Cancellation result.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelReservation(
        int id,
        [FromBody] CancelReservationCommand command)
    {
        var commandWithId = command with { ReservationId = id };
        var result = await _mediator.Send(commandWithId);
        
        if (!result)
            return NotFound($"Reservation {id} not found or cannot be cancelled");
            
        return NoContent();
    }

    /// <summary>
    /// Search reservation records by visitor ID with filtering options.
    /// </summary>
    /// <param name="id">Visitor ID.</param>
    /// <param name="query">Search parameters.</param>
    /// <returns>Paginated reservation results.</returns>
    [HttpGet("visitor/{id}/search")]
    public async Task<ActionResult<ReservationSearchResult>> SearchByVisitor(
        [FromRoute] int id,
        [FromQuery] SearchReservationByVisitorQuery query)
    {
        var queryWithVisitorId = query with { VisitorId = id };
        var result = await _mediator.Send(queryWithVisitorId);
        return Ok(result);
    }

    /// <summary>
    /// Search reservation records by multiple criteria (admin use).
    /// </summary>
    /// <param name="query">Search parameters.</param>
    /// <returns>Paginated reservation results.</returns>
    [HttpGet("search")]
    public async Task<ActionResult<ReservationSearchResult>> Search(
        [FromQuery] SearchReservationQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get reservation record statistics for a visitor.
    /// </summary>
    /// <param name="id">Visitor ID.</param>
    /// <param name="query">Statistics parameters.</param>
    /// <returns>Visitor reservation statistics.</returns>
    [HttpGet("visitor/{id}/stats")]
    public async Task<ActionResult<ReservationStatsDto>> GetVisitorStats(
        [FromRoute] int id,
        [FromQuery] GetVisitorReservationStatsQuery query)
    {
        var queryWithVisitorId = query with { VisitorId = id };
        var result = await _mediator.Send(queryWithVisitorId);
        return Ok(result);
    }
}
