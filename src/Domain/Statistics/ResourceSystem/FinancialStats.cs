namespace DbApp.Domain.Statistics.ResourceSystem;

/// <summary>
/// Overall financial statistics.
/// </summary>
public class FinancialStats
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
public class GroupedFinancialStats
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
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
public class IncomeExpenseOverview
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public int IncomeTransactionCount { get; set; }
    public int ExpenseTransactionCount { get; set; }
    public decimal AverageIncomeAmount { get; set; }
    public decimal AverageExpenseAmount { get; set; }
}
