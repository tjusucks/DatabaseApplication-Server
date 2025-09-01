
using DbApp.Application.TicketingSystem.Promotions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/promotion-actions")]
public class PromotionActionController : ControllerBase
{
    private readonly IMediator _mediator;

    public PromotionActionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Updates an existing promotion action.
    /// </summary>
    [HttpPut("{actionId:int}")]
    public async Task<ActionResult<PromotionActionDto>> UpdateAction(int actionId, [FromBody] CreateActionRequest dto)
    {
        var command = new UpdatePromotionActionCommand(actionId, dto);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound($"Action with ID {actionId} not found.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes a promotion action.
    /// </summary>
    [HttpDelete("{actionId:int}")]
    public async Task<IActionResult> DeleteAction(int actionId)
    {
        var command = new DeletePromotionActionCommand(actionId);
        var success = await _mediator.Send(command);

        if (!success)
        {
            return NotFound($"Action with ID {actionId} not found.");
        }

        return NoContent();
    }
}