
using DbApp.Application.TicketingSystem.Promotions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/promotion-conditions")]
public class PromotionConditionController : ControllerBase
{
    private readonly IMediator _mediator;

    public PromotionConditionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Updates an existing promotion condition.
    /// </summary>
    [HttpPut("{conditionId:int}")]
    public async Task<ActionResult<PromotionConditionDto>> UpdateCondition(int conditionId, [FromBody] CreateConditionRequest dto)
    {
        var command = new UpdatePromotionConditionCommand(conditionId, dto);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound($"Condition with ID {conditionId} not found.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes a promotion condition.
    /// </summary>
    [HttpDelete("{conditionId:int}")]
    public async Task<IActionResult> DeleteCondition(int conditionId)
    {
        var command = new DeletePromotionConditionCommand(conditionId);
        var success = await _mediator.Send(command);

        if (!success)
        {
            return NotFound($"Condition with ID {conditionId} not found.");
        }

        return NoContent();
    }
}