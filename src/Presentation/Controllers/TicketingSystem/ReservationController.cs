using DbApp.Application.TicketingSystem.Reservations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/ticketing/reservations")]
public class ReservationRecordController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Search reservations by visitor ID with filtering options.
    /// </summary>
    /// <param name="visitorId"> Visitor ID </param>
    /// <param name="query"> Search parameters </param>
    /// <returns> Paginated reservation results </returns>
    [HttpGet("visitor/{visitorId}")]
    public async Task<ActionResult<SearchReservationsResult>> SearchByVisitor(
        int visitorId,
        [FromQuery] SearchReservationsByVisitorQuery query)
    {
        var queryWithVisitorId = query with { VisitorId = visitorId };
        var result = await _mediator.Send(queryWithVisitorId);
        return Ok(result);
    }

    /// <summary>
    /// Search reservations by multiple criteria (admin use).
    /// </summary>
    /// <param name="query"> Search parameters </param>
    /// <returns> Paginated reservation results </returns>
    [HttpGet]
    public async Task<ActionResult<SearchReservationsResult>> Search(
        [FromQuery] SearchReservationsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get reservation statistics for a visitor.
    /// </summary>
    /// <param name="visitorId"> Visitor ID </param>
    /// <param name="query"> Statistics parameters </param>
    /// <returns> Visitor reservation statistics </returns>
    [HttpGet("visitor/{visitorId}/stats")]
    public async Task<ActionResult<ReservationStatsDto>> GetVisitorStats(
        int visitorId,
        [FromQuery] GetVisitorReservationStatsQuery query)
    {
        var queryWithVisitorId = query with { VisitorId = visitorId };
        var result = await _mediator.Send(queryWithVisitorId);
        return Ok(result);
    }
}
