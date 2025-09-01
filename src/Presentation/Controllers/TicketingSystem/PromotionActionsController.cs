
using DbApp.Application.TicketingSystem.Promotions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/promotions/{promotionId:int}/actions")]
public class PromotionActionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PromotionActionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Adds a new action to an existing promotion.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PromotionActionDto>> AddAction(int promotionId, [FromBody] CreateActionRequest dto)
    {
        var command = new AddActionToPromotionCommand(promotionId, dto);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound($"Promotion with ID {promotionId} not found.");
        }

        return Ok(result);
    }
}