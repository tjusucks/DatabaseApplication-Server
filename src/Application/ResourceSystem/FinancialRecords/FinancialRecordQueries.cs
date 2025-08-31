using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.FinancialRecords;

/// <summary>
/// Search financial records with filtering and pagination.
/// </summary>
public record SearchFinancialRecordQuery(
    string? Keyword = null, // Search in employee names, staff numbers.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    TransactionType? TransactionType = null,
    PaymentMethod? PaymentMethod = null,
    int? ResponsibleEmployeeId = null,
    int? ApprovedById = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    string? SortBy = "TransactionDate", // Sort by "TransactionDate", "Amount", "CreatedAt".
    bool Descending = true,
    int Page = 1,
    int PageSize = 20
) : IRequest<FinancialRecordResult>;

/// <summary>
/// Get detailed financial record by ID.
/// </summary>
public record GetFinancialRecordByIdQuery(
    int RecordId
) : IRequest<FinancialRecordDetailDto?>;

/// <summary>
/// Get overall financial statistics with filtering.
/// </summary>
public record GetFinancialStatsQuery(
    string? Keyword = null, // Search in employee names, staff numbers.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    TransactionType? TransactionType = null,
    PaymentMethod? PaymentMethod = null,
    int? ResponsibleEmployeeId = null,
    int? ApprovedById = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null
) : IRequest<FinancialStatsDto>;

/// <summary>
/// Get financial statistics grouped by different parameters with filtering.
/// </summary>
public record GetGroupedFinancialStatsQuery(
    string? Keyword = null, // Search in employee names, staff numbers.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    TransactionType? TransactionType = null,
    PaymentMethod? PaymentMethod = null,
    int? ResponsibleEmployeeId = null,
    int? ApprovedById = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    string GroupBy = "TransactionType", // Group by "TransactionType", "PaymentMethod", "Date", "Month", "ResponsibleEmployee".
    string? SortBy = "NetProfit", // Sort by "NetProfit", "TotalIncome", "TotalExpense", "TransactionCount", "GroupName".
    bool Descending = true
) : IRequest<List<GroupedFinancialStatsDto>>;

/// <summary>
/// Get income and expense overview statistics.
/// </summary>
public record GetIncomeExpenseOverviewQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? ResponsibleEmployeeId = null,
    int? ApprovedById = null
) : IRequest<IncomeExpenseOverviewDto>;

/// <summary>
/// Get financial records by transaction type (Income or Expense).
/// </summary>
public record GetFinancialRecordsByTypeQuery(
    TransactionType TransactionType,
    string? Keyword = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    PaymentMethod? PaymentMethod = null,
    int? ResponsibleEmployeeId = null,
    int? ApprovedById = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    string? SortBy = "TransactionDate",
    bool Descending = true,
    int Page = 1,
    int PageSize = 20
) : IRequest<FinancialRecordResult>;
