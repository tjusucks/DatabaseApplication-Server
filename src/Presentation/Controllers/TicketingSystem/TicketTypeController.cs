using DbApp.Application.TicketingSystem.TicketTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/ticketing/ticket-types")]
public class TicketTypeController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<TicketTypeDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllTicketTypesQuery());
        return Ok(result);
    }

    [HttpGet("{ticketTypeId:int}")]
    public async Task<ActionResult<TicketTypeDto>> GetById(int ticketTypeId)
    {
        var type = await _mediator.Send(new GetTicketTypeByIdQuery(ticketTypeId));
        return Ok(type);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateTicketTypeCommand command)
    {
        var newTicketTypeId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { ticketTypeId = newTicketTypeId }, null);
    }

    [HttpPut("{ticketTypeId:int}")]
    public async Task<IActionResult> Update(int ticketTypeId, [FromBody] UpdateTicketTypeCommand command)
    {
        if (ticketTypeId != command.TicketTypeId)
        {
            return BadRequest("TicketTypeId mismatch.");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{ticketTypeId:int}")]
    public async Task<IActionResult> Delete(int ticketTypeId)
    {
        await _mediator.Send(new DeleteTicketTypeCommand(ticketTypeId));
        return NoContent();
    }
}
