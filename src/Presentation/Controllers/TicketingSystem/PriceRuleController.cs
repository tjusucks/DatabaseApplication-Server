using DbApp.Application.TicketingSystem.PriceRules;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/ticketing/ticket-types/{ticketTypeId:int}/price-rules")]
public class PriceRuleController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreatePriceRuleCommand command)
    {
        var newRuleId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetRuleById), new { ticketTypeId = command.TicketTypeId, ruleId = newRuleId }, null);
    }

    [HttpGet]
    public async Task<ActionResult<List<PriceRuleDto>>> GetRules(int ticketTypeId)
    {
        var result = await _mediator.Send(new GetPriceRuleByTicketTypeQuery(ticketTypeId));
        return Ok(result);
    }

    [HttpGet("{ruleId:int}")]
    public async Task<ActionResult<PriceRuleDto>> GetRuleById(int ticketTypeId, int ruleId)
    {
        var rule = await _mediator.Send(new GetPriceRuleByIdQuery(ticketTypeId, ruleId));
        if (rule == null)
        {
            return NotFound($"Price rule with ID {ruleId} not found.");
        }
        return Ok(rule);
    }

    [HttpPut("{ruleId:int}")]
    public async Task<IActionResult> Update(int ruleId, [FromBody] UpdatePriceRuleCommand command)
    {
        if (command.RuleId != ruleId)
        {
            return BadRequest("RuleId mismatch.");
        }
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{ruleId:int}")]
    public async Task<IActionResult> Delete(int ruleId)
    {
        await _mediator.Send(new DeletePriceRuleCommand(ruleId));
        return NoContent();
    }
}
