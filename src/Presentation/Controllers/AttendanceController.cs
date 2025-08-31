using DbApp.Application.ResourceSystem.Attendances;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
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

        // 根据ID获取考勤记录
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var attendance = await _mediator.Send(new GetAttendanceByIdQuery(id));
                return attendance != null ? Ok(attendance) : NotFound();
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

        // 获取员工考勤记录
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeAttendance(
            int employeeId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _mediator.Send(new GetEmployeeAttendanceQuery(employeeId, startDate, endDate));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 获取员工考勤统计
        [HttpGet("employee/{employeeId}/stats")]
        public async Task<IActionResult> GetEmployeeStats(
            int employeeId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _mediator.Send(new GetEmployeeStatsQuery(employeeId, startDate, endDate));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 获取员工月度考勤统计
        [HttpGet("employee/{employeeId}/monthly/{year}/{month}")]
        public async Task<IActionResult> GetEmployeeMonthlyStats(
            int employeeId, int year, int month)
        {
            try
            {
                var result = await _mediator.Send(new GetEmployeeMonthlyStatsQuery(employeeId, year, month));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 获取部门考勤记录
        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetDepartmentAttendance(
            string departmentId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _mediator.Send(new GetDepartmentAttendanceQuery(departmentId, startDate, endDate));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 获取部门考勤统计
        [HttpGet("department/{departmentId}/stats")]
        public async Task<IActionResult> GetDepartmentStats(
            string departmentId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _mediator.Send(new GetDepartmentStatsQuery(departmentId, startDate, endDate));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 获取异常考勤记录
        [HttpGet("abnormal")]
        public async Task<IActionResult> GetAbnormalRecords(
            [FromQuery] int? employeeId, 
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _mediator.Send(new GetAbnormalRecordsQuery(employeeId, startDate, endDate));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}