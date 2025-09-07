using DbApp.Application.ResourceSystem.EmployeeReviews;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/employee-reviews")]
public class EmployeeReviewController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

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

    // 统一查询端点 - 根据不同参数返回不同结果
    [HttpGet("search")]
    public async Task<IActionResult> GetEmployeeReviews(
        [FromQuery] int? id = null,
        [FromQuery] int? employeeId = null,
        [FromQuery] string? period = null,
        [FromQuery] int? evaluatorId = null,
        [FromQuery] int? year = null,
        [FromQuery] int? month = null,
        [FromQuery] int? quarter = null,
        [FromQuery] bool statistics = false)
    {
        try
        {
            // 根据ID获取特定的员工绩效记录
            if (id.HasValue)
            {
                var review = await _mediator.Send(new GetEmployeeReviewByIdQuery(id.Value));
                return review != null ? Ok(review) : NotFound();
            }

            // 获取员工绩效统计信息
            if (employeeId.HasValue && year.HasValue && statistics)
            {
                var statisticsResult = await _mediator.Send(new GetEmployeeReviewStatisticsQuery(employeeId.Value, year.Value, month, quarter));
                return Ok(statisticsResult);
            }

            // 根据员工ID和考核周期获取绩效记录
            if (employeeId.HasValue && !string.IsNullOrEmpty(period))
            {
                var review = await _mediator.Send(new GetEmployeeReviewByEmployeeAndPeriodQuery(employeeId.Value, period));
                return review != null ? Ok(review) : NotFound();
            }

            // 根据员工ID获取绩效记录
            if (employeeId.HasValue)
            {
                var reviews = await _mediator.Send(new GetEmployeeReviewsByEmployeeQuery(employeeId.Value));
                return Ok(reviews);
            }

            // 根据考核周期获取绩效记录
            if (!string.IsNullOrEmpty(period))
            {
                var reviews = await _mediator.Send(new GetEmployeeReviewsByPeriodQuery(period));
                return Ok(reviews);
            }

            // 根据评估人ID获取绩效记录
            if (evaluatorId.HasValue)
            {
                var reviews = await _mediator.Send(new GetEmployeeReviewsByEvaluatorQuery(evaluatorId.Value));
                return Ok(reviews);
            }

            // 获取所有员工绩效记录
            var allReviews = await _mediator.Send(new GetAllEmployeeReviewsQuery());
            return Ok(allReviews);
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

    // 保留原有的GetById方法以支持CreatedAtAction
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
}
