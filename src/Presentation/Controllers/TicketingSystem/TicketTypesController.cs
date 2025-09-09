using DbApp.Application.TicketingSystem.Reservations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.TicketingSystem;

/// <summary>
/// 票种管理API控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TicketTypesController(IMediator mediator, ILogger<TicketTypesController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<TicketTypesController> _logger = logger;

    /// <summary>
    /// 获取所有可用票种
    /// </summary>
    /// <param name="forDate">可选：查询特定日期的票种可用性</param>
    /// <returns>可用票种列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<TicketTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TicketTypeDto>>> GetAvailableTicketTypes([FromQuery] DateTime? forDate = null)
    {
        try
        {
            var query = new GetAvailableTicketTypesQuery { ForDate = forDate };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available ticket types");
            return StatusCode(500, "获取票种信息时发生错误");
        }
    }

    /// <summary>
    /// 根据ID获取特定票种详情
    /// </summary>
    /// <param name="id">票种ID</param>
    /// <returns>票种详情</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TicketTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketTypeDto>> GetTicketType(int id)
    {
        try
        {
            var query = new GetTicketTypeByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound($"票种ID {id} 不存在");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ticket type {TicketTypeId}", id);
            return StatusCode(500, "获取票种详情时发生错误");
        }
    }
}
