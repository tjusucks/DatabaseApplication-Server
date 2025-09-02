using System;
using System.Threading.Tasks;
using DbApp.Application.ResourceSystem.EmployeeReviews;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/employeereviews")]
    public class EmployeeReviewController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 创建员工绩效记录
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeReviewCommand command)
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

        // 更新员工绩效记录
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeReviewCommand command)
        {
            try
            {
                if (id != command.ReviewId)
                    return BadRequest("URL ID与请求体ID不匹配");

                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 删除员工绩效记录
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _mediator.Send(new DeleteEmployeeReviewCommand(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 根据ID获取员工绩效记录
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var review = await _mediator.Send(new GetEmployeeReviewByIdQuery(id));
                return review != null ? Ok(review) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 获取所有员工绩效记录
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var reviews = await _mediator.Send(new GetAllEmployeeReviewsQuery());
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 根据员工ID获取绩效记录
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployee(int employeeId)
        {
            try
            {
                var reviews = await _mediator.Send(new GetEmployeeReviewsByEmployeeQuery(employeeId));
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 根据考核周期获取绩效记录
        [HttpGet("period/{period}")]
        public async Task<IActionResult> GetByPeriod(string period)
        {
            try
            {
                var reviews = await _mediator.Send(new GetEmployeeReviewsByPeriodQuery(period));
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 根据评估人ID获取绩效记录
        [HttpGet("evaluator/{evaluatorId}")]
        public async Task<IActionResult> GetByEvaluator(int evaluatorId)
        {
            try
            {
                var reviews = await _mediator.Send(new GetEmployeeReviewsByEvaluatorQuery(evaluatorId));
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 根据员工ID和考核周期获取绩效记录
        [HttpGet("employee/{employeeId}/period/{period}")]
        public async Task<IActionResult> GetByEmployeeAndPeriod(int employeeId, string period)
        {
            try
            {
                var review = await _mediator.Send(new GetEmployeeReviewByEmployeeAndPeriodQuery(employeeId, period));
                return review != null ? Ok(review) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 获取员工绩效统计信息
        [HttpGet("employee/{employeeId}/statistics")]
        public async Task<IActionResult> GetEmployeeStatistics(int employeeId, int year, int? month = null, int? quarter = null)
        {
            try
            {
                var statistics = await _mediator.Send(new GetEmployeeReviewStatisticsQuery(employeeId, year, month, quarter));
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("attendance-deduction")]
        public async Task<IActionResult> CreateAttendanceDeduction([FromBody] CreateAttendanceDeductionCommand command)
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

    }
}
