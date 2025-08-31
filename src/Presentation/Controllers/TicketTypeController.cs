using DbApp.Application.TicketingSystem.TicketTypes; 
using MediatR; 
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbApp.Presentation.Controllers;

[ApiController]
[Route("api/ticket-types")]
public class TicketTypeController : ControllerBase
{
    private readonly IMediator _mediator; // Inject IMediator

    public TicketTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all ticket types.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TicketTypeSummaryDto>>> GetAll()
    {
        var query = new GetAllTicketTypesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a ticket type by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketTypeSummaryDto>> GetById(int id)
    {
        var query = new GetTicketTypeByIdQuery(id); // Assuming you created this query
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound($"Ticket type with ID {id} not found.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Update the base price of a ticket type.
    /// </summary>
    [HttpPut("{id:int}/base-price")]
    public async Task<IActionResult> UpdateBasePrice(int id, [FromBody] UpdateBasePriceRequest dto)
    {
        var command = new UpdateBasePriceCommand(id, dto);
        var success = await _mediator.Send(command);

        if (!success)
        {
            // The handler can return false for validation errors (e.g., ticket not found)
            return BadRequest("Failed to update base price. Check ticket ID and price value.");
        }

        return NoContent(); // Correct response for a successful update with no content to return
    }
}