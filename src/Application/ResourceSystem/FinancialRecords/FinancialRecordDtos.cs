using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Application.ResourceSystem.FinancialRecords;

/// <summary>
/// Summary information for financial record.
/// </summary>
public class FinancialRecordSummaryDto
{
    public int RecordId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? ResponsibleEmployeeName { get; set; }
    public string? ApprovedByName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Detailed information for financial record.
/// </summary>
public class FinancialRecordDetailDto
{
    public int RecordId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public int? ResponsibleEmployeeId { get; set; }
    public string? ResponsibleEmployeeName { get; set; }
    public string? ResponsibleEmployeeStaffNumber { get; set; }
    public int? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public string? ApprovedByStaffNumber { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Paginated search results for financial records.
/// </summary>
public class FinancialRecordResult
{
    public List<FinancialRecordSummaryDto> FinancialRecords { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// Overall financial statistics.
/// </summary>
public class FinancialStatsDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal TotalRefund { get; set; }
    public decimal TotalTransfer { get; set; }
    public decimal NetProfit { get; set; }
    public int TotalRecords { get; set; }
    public decimal AverageTransactionAmount { get; set; }
    public DateTime? FirstTransaction { get; set; }
    public DateTime? LastTransaction { get; set; }
}

/// <summary>
/// Grouped financial statistics by different dimensions.
/// </summary>
public class GroupedFinancialStatsDto
{
    public string GroupKey { get; set; } = string.Empty; // The value used for grouping.
    public string GroupName { get; set; } = string.Empty; // Display name for the group.
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal TotalRefund { get; set; }
    public decimal TotalTransfer { get; set; }
    public decimal NetProfit { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageAmount { get; set; }
    public DateTime? FirstTransaction { get; set; }
    public DateTime? LastTransaction { get; set; }
}

/// <summary>
/// Income and expense overview statistics.
/// </summary>
public class IncomeExpenseOverviewDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ProfitMargin { get; set; } // Net profit / Total income * 100
    public int IncomeTransactionCount { get; set; }
    public int ExpenseTransactionCount { get; set; }
    public decimal AverageIncomeAmount { get; set; }
    public decimal AverageExpenseAmount { get; set; }
}
