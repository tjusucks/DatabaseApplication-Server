using DbApp.Application.ResourceSystem.FinancialRecords;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DbApp.Presentation.Controllers.ResourceSystem;

[ApiController]
[Route("api/resource/financial-records")]
public class FinancialRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Create a new financial record.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FinancialRecordDetailDto>> CreateFinancialRecord([FromBody] CreateFinancialRecordCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetFinancialRecord), new { id = result.RecordId }, result);
    }

    /// <summary>
    /// Get financial record by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FinancialRecordDetailDto>> GetFinancialRecord(int id)
    {
        var query = new GetFinancialRecordByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound($"Financial record with ID {id} not found.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Search financial records with filtering and pagination.
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<FinancialRecordResult>> SearchFinancialRecords([FromQuery] SearchFinancialRecordQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update an existing financial record.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<FinancialRecordDetailDto>> UpdateFinancialRecord(int id, [FromBody] UpdateFinancialRecordCommand command)
    {
        if (id != command.RecordId)
        {
            return BadRequest("Record ID in URL does not match command.");
        }

        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound($"Financial record with ID {id} not found.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete a financial record.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteFinancialRecord(int id)
    {
        var command = new DeleteFinancialRecordCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
        {
            return NotFound($"Financial record with ID {id} not found.");
        }

        return NoContent();
    }

    /// <summary>
    /// Get overall financial statistics.
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<FinancialStatsDto>> GetFinancialStats([FromQuery] GetFinancialStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get grouped financial statistics.
    /// </summary>
    [HttpGet("stats/grouped")]
    public async Task<ActionResult<List<GroupedFinancialStatsDto>>> GetGroupedFinancialStats([FromQuery] GetGroupedFinancialStatsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get income and expense overview.
    /// </summary>
    [HttpGet("overview")]
    public async Task<ActionResult<IncomeExpenseOverviewDto>> GetIncomeExpenseOverview([FromQuery] GetIncomeExpenseOverviewQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get financial records by transaction type (Income or Expense).
    /// </summary>
    [HttpGet("by-type/{transactionType}")]
    public async Task<ActionResult<FinancialRecordResult>> GetFinancialRecordsByType(
        TransactionType transactionType,
        [FromQuery] string? keyword = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] PaymentMethod? paymentMethod = null,
        [FromQuery] int? responsibleEmployeeId = null,
        [FromQuery] int? approvedById = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        [FromQuery] string? sortBy = "TransactionDate",
        [FromQuery] bool descending = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetFinancialRecordsByTypeQuery(
            transactionType,
            keyword,
            startDate,
            endDate,
            paymentMethod,
            responsibleEmployeeId,
            approvedById,
            minAmount,
            maxAmount,
            sortBy,
            descending,
            page,
            pageSize);

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
