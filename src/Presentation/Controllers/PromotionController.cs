using DbApp.Application.TicketingSystem.Promotions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbApp.Presentation.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionController : ControllerBase
{
    private readonly IMediator _mediator;

    public PromotionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<PromotionSummaryDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllPromotionsQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PromotionDetailDto>> GetDetail(int id)
    {
        var detail = await _mediator.Send(new GetPromotionDetailQuery(id));
        if (detail == null)
        {
            return NotFound($"Promotion with ID {id} not found.");
        }
        return Ok(detail);
    }

    [HttpPost]
    public async Task<ActionResult<PromotionDetailDto>> Create([FromBody] CreatePromotionRequest dto)
    {
        var promotion = await _mediator.Send(new CreatePromotionCommand(dto));
        return CreatedAtAction(nameof(GetDetail), new { id = promotion.Id }, promotion);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PromotionDetailDto>> Update(int id, [FromBody] UpdatePromotionRequest dto)
    {
        var promotion = await _mediator.Send(new UpdatePromotionCommand(id, dto));
        if (promotion == null)
        {
            return NotFound($"Promotion with ID {id} not found.");
        }
        return Ok(promotion);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _mediator.Send(new DeletePromotionCommand(id));
        if (!success)
        {
            return NotFound($"Promotion with ID {id} not found.");
        }
        return NoContent();
    }
}