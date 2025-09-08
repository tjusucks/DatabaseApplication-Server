using DbApp.Application.TicketingSystem.Promotions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.TicketingSystem;

[ApiController]
[Route("api/ticketing/promotions")]
public class PromotionController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<PromotionDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllPromotionsQuery());
        return Ok(result);
    }

    [HttpGet("{promotionId:int}")]
    public async Task<ActionResult<PromotionDto>> GetById(int promotionId)
    {
        var promotion = await _mediator.Send(new GetPromotionByIdQuery(promotionId));
        return Ok(promotion);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreatePromotionCommand command)
    {
        var newPromotionId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { promotionId = newPromotionId }, null);
    }

    [HttpPut("{promotionId:int}")]
    public async Task<IActionResult> Update(int promotionId, [FromBody] UpdatePromotionCommand command)
    {
        if (promotionId != command.PromotionId)
        {
            return BadRequest("PromotionId mismatch.");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{promotionId:int}")]
    public async Task<IActionResult> Delete(int promotionId)
    {
        await _mediator.Send(new DeletePromotionCommand(promotionId));
        return NoContent();
    }
}
