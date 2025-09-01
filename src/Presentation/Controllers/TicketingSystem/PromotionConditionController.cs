using DbApp.Application.TicketingSystem.PromotionConditions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/ticketing/promotions/{promotionId:int}/conditions")]
public class PromotionConditionController : ControllerBase
{
    private readonly IMediator _mediator;

    public PromotionConditionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<PromotionConditionDto>>> GetConditions(int promotionId)
    {
        var result = await _mediator.Send(new GetPromotionConditionsByPromotionIdQuery(promotionId));
        return Ok(result);
    }

    [HttpGet("{conditionId:int}")]
    public async Task<ActionResult<PromotionConditionDto>> GetConditionById(int promotionId, int conditionId)
    {
        var result = await _mediator.Send(new GetPromotionConditionByIdQuery(promotionId, conditionId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreatePromotionConditionCommand command)
    {
        var newConditionId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetConditionById), new { promotionId = command.PromotionId, conditionId = newConditionId }, null);
    }

    [HttpPut("{conditionId:int}")]
    public async Task<IActionResult> Update(int conditionId, [FromBody] UpdatePromotionConditionCommand command)
    {
        if (conditionId != command.ConditionId)
        {
            return BadRequest("Condition ID mismatch");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{conditionId:int}")]
    public async Task<IActionResult> Delete(int conditionId)
    {
        await _mediator.Send(new DeletePromotionConditionCommand(conditionId));
        return NoContent();
    }
}
