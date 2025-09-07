using DbApp.Application.TicketingSystem.Tickets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/ticketing/tickets")]
public class TicketController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Search ticket sales with filtering and pagination.
    /// </summary>
    /// <param name="query">Search parameters.</param>
    /// <returns>Paginated ticket sales results.</returns>
    [HttpGet("sales/search")]
    public async Task<ActionResult<TicketSaleResult>> SearchTicketSales(
        [FromQuery] SearchTicketSaleQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get ticket sales statistics.
    /// </summary>
    /// <param name="query">Statistics parameters.</param>
    /// <returns>Ticket sales statistics.</returns>
    [HttpGet("sales/stats")]
    public async Task<ActionResult<TicketSaleStatsDto>> GetTicketSalesStats(
        [FromQuery] GetTicketSaleStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get grouped ticket sales statistics.
    /// </summary>
    /// <param name="query">Grouped statistics parameters.</param>
    /// <returns>Grouped ticket sales statistics.</returns>
    [HttpGet("sales/stats/grouped")]
    public async Task<ActionResult<List<GroupedTicketSaleStatsDto>>> GetGroupedTicketSalesStats(
        [FromQuery] GetGroupedTicketSaleStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
