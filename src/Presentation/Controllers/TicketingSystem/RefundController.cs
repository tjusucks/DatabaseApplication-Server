using DbApp.Application.TicketingSystem.Refunds;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.TicketingSystem
{
    /// <summary>
    /// 退票功能控制器
    /// </summary>
    [ApiController]
    [Route("api/ticketing/refunds")]
    public class RefundController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// 申请退票
        /// </summary>
        /// <param name="command">退票申请参数</param>
        /// <returns>退票结果</returns>
        [HttpPost("request")]
        [ProducesResponseType(typeof(RefundResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestRefund([FromBody] RequestRefundCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// 处理退票申请（管理员）
        /// </summary>
        /// <param name="refundId">退票记录ID</param>
        /// <param name="command">处理参数</param>
        /// <returns>处理结果</returns>
        [HttpPost("{refundId}/process")]
        [ProducesResponseType(typeof(RefundResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessRefund([FromBody] ProcessRefundCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// 批量退票（管理员）
        /// </summary>
        /// <param name="command">批量退票参数</param>
        /// <returns>批量处理结果</returns>
        [HttpPost("batch")]
        [ProducesResponseType(typeof(BatchRefundResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BatchRefund([FromBody] BatchRefundCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// 搜索退票记录
        /// </summary>
        /// <param name="query">搜索参数</param>
        /// <returns>退票记录列表</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(RefundSearchResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchRefunds([FromQuery] SearchRefundRecordsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// 根据ID获取退票记录详情
        /// </summary>
        /// <param name="id">退票记录ID</param>
        /// <returns>退票记录详情</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RefundDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRefundById(int id)
        {
            var query = new GetRefundByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound($"Refund record {id} not found");

            return Ok(result);
        }

        /// <summary>
        /// 根据票ID获取退票记录
        /// </summary>
        /// <param name="ticketId">票ID</param>
        /// <returns>退票记录详情</returns>
        [HttpGet("by-ticket/{ticketId}")]
        [ProducesResponseType(typeof(RefundDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRefundByTicketId(int ticketId)
        {
            var query = new GetRefundByTicketIdQuery(ticketId);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound($"No refund record found for ticket {ticketId}");

            return Ok(result);
        }

        /// <summary>
        /// 获取退票统计信息
        /// </summary>
        /// <param name="query">统计查询参数</param>
        /// <returns>退票统计</returns>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(RefundStatsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRefundStats([FromQuery] GetRefundStatsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// 检查票是否可以退票
        /// </summary>
        /// <param name="ticketId">票ID</param>
        /// <returns>是否可以退票</returns>
        [HttpGet("check/{ticketId}")]
        public ActionResult<bool> CanRefundTicket(int ticketId)
        {
            // 这里可以添加一个专门的查询来检查退票资格
            // 暂时返回简单的响应
            return Ok(true);
        }
    }
}
