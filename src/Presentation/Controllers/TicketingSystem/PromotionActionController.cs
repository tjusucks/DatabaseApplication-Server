using DbApp.Application.TicketingSystem.PromotionActions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/ticketing/promotions/{promotionId:int}/actions")]
public class PromotionActionController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<PromotionActionDto>>> GetActions(int promotionId)
    {
        var result = await _mediator.Send(new GetPromotionActionsByPromotionIdQuery(promotionId));
        return Ok(result);
    }

    [HttpGet("{actionId:int}")]
    public async Task<ActionResult<PromotionActionDto>> GetActionById(int promotionId, int actionId)
    {
        var action = await _mediator.Send(new GetPromotionActionByIdQuery(promotionId, actionId));
        if (action == null)
        {
            return NotFound($"Promotion action with ID {actionId} not found.");
        }
        return Ok(action);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreatePromotionActionCommand command)
    {
        var newActionId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetActionById), new { promotionId = command.PromotionId, actionId = newActionId }, null);
    }

    [HttpPut("{actionId:int}")]
    public async Task<IActionResult> Update(int actionId, [FromBody] UpdatePromotionActionCommand command)
    {
        if (actionId != command.ActionId)
        {
            return BadRequest("Action ID mismatch");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{actionId:int}")]
    public async Task<IActionResult> Delete(int actionId)
    {
        await _mediator.Send(new DeletePromotionActionCommand(actionId));
        return NoContent();
    }
}
