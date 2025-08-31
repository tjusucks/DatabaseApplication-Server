using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Application.ResourceSystem.SalaryRecords;

/// <summary>
/// Summary information for salary record.
/// </summary>
public class SalaryRecordSummaryDto
{
    public int SalaryRecordId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string StaffNumber { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime PayDate { get; set; }
    public decimal Salary { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Detailed information for salary record.
/// </summary>
public class SalaryRecordDetailDto
{
    public int SalaryRecordId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string StaffNumber { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? DepartmentName { get; set; }
    public StaffType? StaffType { get; set; }
    public EmploymentStatus EmploymentStatus { get; set; }
    public DateTime PayDate { get; set; }
    public decimal Salary { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Paginated search results for salary records.
/// </summary>
public class SalaryRecordResult
{
    public List<SalaryRecordSummaryDto> SalaryRecords { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// Overall salary statistics.
/// </summary>
public class SalaryStatsDto
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
public class GroupedSalaryStatsDto
{
    public string GroupKey { get; set; } = string.Empty; // The value used for grouping.
    public string GroupName { get; set; } = string.Empty; // Display name for the group.
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
public class EmployeeSalarySummaryDto
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
public class MonthlySalaryReportDto
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
