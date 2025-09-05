using System;
using System.Threading.Tasks;
using DbApp.Application.ResourceSystem.Attendances;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AttendanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 创建考勤记录
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAttendanceCommand command)
        {
            try
            {
                var id = await _mediator.Send(command);
                return CreatedAtAction(nameof(GenericQuery), new { id }, new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 更新考勤记录
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAttendanceCommand command)
        {
            try
            {
                if (id != command.AttendanceId)
                    return BadRequest("URL ID与请求体ID不匹配");

                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 删除考勤记录
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _mediator.Send(new DeleteAttendanceCommand(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // 员工签到
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] RecordCheckInCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 员工签退
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] RecordCheckOutCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 申请请假
        [HttpPost("leave")]
        public async Task<IActionResult> ApplyLeave([FromBody] ApplyLeaveCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 更新考勤状态
        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAttendanceStatusCommand command)
        {
            try
            {
                if (id != command.AttendanceId)
                    return BadRequest("URL ID与请求体ID不匹配");

                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 统一查询接口
        [HttpPost("query")]
        public async Task<IActionResult> GenericQuery([FromBody] GenericAttendanceQueryRequest request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
