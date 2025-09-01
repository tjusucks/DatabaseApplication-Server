
using DbApp.Application.TicketingSystem.Promotions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/promotions/{promotionId:int}/conditions")]
public class PromotionConditionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PromotionConditionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Adds a new condition to an existing promotion.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PromotionConditionDto>> AddCondition(int promotionId, [FromBody] CreateConditionRequest dto)
    {
        var command = new AddConditionToPromotionCommand(promotionId, dto);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound($"Promotion with ID {promotionId} not found.");
        }

        // You'd ideally have a GetById endpoint for conditions to return a CreatedAtAction result.
        return Ok(result);
    }
}