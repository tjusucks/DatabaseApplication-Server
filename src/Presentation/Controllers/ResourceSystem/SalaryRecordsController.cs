using DbApp.Application.ResourceSystem.SalaryRecords;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/salary-records")]
public class SalaryRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Create a new salary record.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SalaryRecordDetailDto>> CreateSalaryRecord([FromBody] CreateSalaryRecordCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSalaryRecord), new { salaryRecordId = result.SalaryRecordId }, result);
    }

    /// <summary>
    /// Create salary records for multiple employees (batch payroll).
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<List<SalaryRecordDetailDto>>> CreateBatchSalaryRecords([FromBody] CreateBatchSalaryRecordsCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get salary record by ID.
    /// </summary>
    [HttpGet("{salaryRecordId:int}")]
    public async Task<ActionResult<SalaryRecordDetailDto>> GetSalaryRecord(int salaryRecordId)
    {
        var query = new GetSalaryRecordByIdQuery(salaryRecordId);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound($"Salary record with ID {salaryRecordId} not found.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Update an existing salary record.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SalaryRecordDetailDto>> UpdateSalaryRecord(int id, [FromBody] UpdateSalaryRecordCommand command)
    {
        if (id != command.SalaryRecordId)
        {
            return BadRequest("Salary record ID in URL does not match command.");
        }

        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound($"Salary record with ID {id} not found.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete a salary record.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteSalaryRecord(int id)
    {
        var command = new DeleteSalaryRecordCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
        {
            return NotFound($"Salary record with ID {id} not found.");
        }

        return NoContent();
    }

    /// <summary>
    /// Search salary records with filtering and pagination.
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<SalaryRecordResult>> SearchSalaryRecords([FromQuery] SearchSalaryRecordQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get overall salary statistics.
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<SalaryStatsDto>> GetSalaryStats([FromQuery] GetSalaryStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get grouped salary statistics.
    /// </summary>
    [HttpGet("stats/grouped")]
    public async Task<ActionResult<List<GroupedSalaryStatsDto>>> GetGroupedSalaryStats([FromQuery] GetGroupedSalaryStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get salary records for a specific employee.
    /// </summary>
    [HttpGet("employee/{id:int}")]
    public async Task<ActionResult<SalaryRecordResult>> GetEmployeeSalaryRecords(int id, [FromQuery] GetEmployeeSalaryRecordsQuery query)
    {
        if (id != query.EmployeeId)
        {
            return BadRequest("Employee ID in URL does not match query parameter.");
        }

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get salary summary for a specific employee.
    /// </summary>
    [HttpGet("employee/{id:int}/summary")]
    public async Task<ActionResult<EmployeeSalarySummaryDto>> GetEmployeeSalarySummary(
        int id,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = new GetEmployeeSalarySummaryQuery(id, startDate, endDate);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound($"Employee with ID {id} not found or has no salary records.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Get monthly salary report.
    /// </summary>
    [HttpGet("reports/monthly")]
    public async Task<ActionResult<List<MonthlySalaryReportDto>>> GetMonthlySalaryReport([FromQuery] GetMonthlySalaryReportQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get payroll for a specific date.
    /// </summary>
    [HttpGet("payroll")]
    public async Task<ActionResult<List<SalaryRecordSummaryDto>>> GetPayroll([FromQuery] GetPayrollQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
