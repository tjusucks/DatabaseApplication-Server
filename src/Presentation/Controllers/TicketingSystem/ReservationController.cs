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
    /// Search reservation records by visitor ID with filtering options.
    /// </summary>
    /// <param name="visitorId">Visitor ID.</param>
    /// <param name="query">Search parameters.</param>
    /// <returns>Paginated reservation results.</returns>
    [HttpGet("visitor/{visitorId}/search")]
    public async Task<ActionResult<ReservationRecordResult>> SearchByVisitor(
        [FromRoute] int visitorId,
        [FromQuery] SearchReservationRecordByVisitorQuery query)
    {
        var queryWithVisitorId = query with { VisitorId = visitorId };
        var result = await _mediator.Send(queryWithVisitorId);
        return Ok(result);
    }

    /// <summary>
    /// Search reservation records by multiple criteria (admin use).
    /// </summary>
    /// <param name="query">Search parameters.</param>
    /// <returns>Paginated reservation results.</returns>
    [HttpGet("search")]
    public async Task<ActionResult<ReservationRecordResult>> Search(
        [FromQuery] SearchReservationRecordQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get reservation record statistics for a visitor.
    /// </summary>
    /// <param name="visitorId">Visitor ID.</param>
    /// <param name="query">Statistics parameters.</param>
    /// <returns>Visitor reservation statistics.</returns>
    [HttpGet("visitor/{visitorId}/stats/search")]
    public async Task<ActionResult<ReservationRecordStatsDto>> GetVisitorStats(
        [FromRoute] int visitorId,
        [FromQuery] GetVisitorReservationRecordStatsQuery query)
    {
        var queryWithVisitorId = query with { VisitorId = visitorId };
        var result = await _mediator.Send(queryWithVisitorId);
        return Ok(result);
    }
}
