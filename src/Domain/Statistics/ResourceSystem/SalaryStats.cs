namespace DbApp.Domain.Statistics.ResourceSystem;

/// <summary>
/// Overall salary statistics.
/// </summary>
public class SalaryStats
{
    public decimal TotalSalaryPaid { get; set; }
    public decimal AverageSalary { get; set; }
    public decimal HighestSalary { get; set; }
    public decimal LowestSalary { get; set; }
    public int TotalEmployees { get; set; }
    public int TotalRecords { get; set; }
    public DateTime? FirstPayment { get; set; }
    public DateTime? LastPayment { get; set; }
}

/// <summary>
/// Grouped salary statistics by different dimensions.
/// </summary>
public class GroupedSalaryStats
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public decimal TotalSalaryPaid { get; set; }
    public decimal AverageSalary { get; set; }
    public decimal HighestSalary { get; set; }
    public decimal LowestSalary { get; set; }
    public int EmployeeCount { get; set; }
    public int RecordCount { get; set; }
    public DateTime? FirstPayment { get; set; }
    public DateTime? LastPayment { get; set; }
}

/// <summary>
/// Employee salary summary for a specific employee.
/// </summary>
public class EmployeeSalarySummary
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string StaffNumber { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? DepartmentName { get; set; }
    public decimal TotalSalaryReceived { get; set; }
    public decimal AverageSalary { get; set; }
    public decimal LatestSalary { get; set; }
    public DateTime? LatestPayDate { get; set; }
    public int TotalPayments { get; set; }
    public DateTime? FirstPayment { get; set; }
    public DateTime? LastPayment { get; set; }
}

/// <summary>
/// Monthly salary report.
/// </summary>
public class MonthlySalaryReport
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalSalaryPaid { get; set; }
    public int EmployeesPaid { get; set; }
    public decimal AverageSalary { get; set; }
    public decimal HighestSalary { get; set; }
    public decimal LowestSalary { get; set; }
}
