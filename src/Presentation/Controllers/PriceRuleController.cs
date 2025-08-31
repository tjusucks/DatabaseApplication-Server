
using DbApp.Application.TicketingSystem.PriceRules;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbApp.Presentation.Controllers;

[ApiController]
[Route("api/ticket-types/{ticketTypeId:int}/price-rules")]
public class PriceRuleController : ControllerBase
{
    private readonly IMediator _mediator;

    public PriceRuleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<PriceRuleDto>>> GetRules(int ticketTypeId)
    {
        var result = await _mediator.Send(new GetPriceRulesByTicketTypeQuery(ticketTypeId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PriceRuleDto>> Create(int ticketTypeId, [FromBody] CreatePriceRuleRequest dto)
    {
        var rule = await _mediator.Send(new CreatePriceRuleCommand(ticketTypeId, dto));
        if (rule == null)
        {
            return BadRequest("Invalid ticket type or rule parameters.");
        }
        
        // Proper REST response: 201 Created. You would need a GetById endpoint for the location header.
        // Example: return CreatedAtAction(nameof(GetRuleById), new { ruleId = rule.Id }, rule);
        return Ok(rule);
    }

    [HttpPut("{ruleId:int}")]
    public async Task<ActionResult<PriceRuleDto>> Update(int ruleId, [FromBody] UpdatePriceRuleRequest dto)
    {
        var rule = await _mediator.Send(new UpdatePriceRuleCommand(ruleId, dto));
        if (rule == null)
        {
            return NotFound($"Price rule with ID {ruleId} not found or parameters are invalid.");
        }
        return Ok(rule);
    }

    [HttpDelete("{ruleId:int}")]
    public async Task<IActionResult> Delete(int ruleId)
    {
        var success = await _mediator.Send(new DeletePriceRuleCommand(ruleId));
        if (!success)
        {
            return NotFound($"Price rule with ID {ruleId} not found.");
        }
        return NoContent();
    }
}