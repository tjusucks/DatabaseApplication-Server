using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.SalaryRecords;

/// <summary>
/// Search salary records with filtering and pagination.
/// </summary>
public record SearchSalaryRecordQuery(
    string? Keyword = null, // Search in employee names, staff numbers, positions, departments.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? EmployeeId = null,
    string? Position = null,
    string? DepartmentName = null,
    StaffType? StaffType = null,
    EmploymentStatus? EmploymentStatus = null,
    decimal? MinSalary = null,
    decimal? MaxSalary = null,
    string? SortBy = "PayDate", // Sort by "PayDate", "Salary", "EmployeeName", "CreatedAt".
    bool Descending = true,
    int Page = 1,
    int PageSize = 20
) : IRequest<SalaryRecordResult>;

/// <summary>
/// Get detailed salary record by ID.
/// </summary>
public record GetSalaryRecordByIdQuery(
    int SalaryRecordId
) : IRequest<SalaryRecordDetailDto?>;

/// <summary>
/// Get overall salary statistics with filtering.
/// </summary>
public record GetSalaryStatsQuery(
    string? Keyword = null, // Search in employee names, staff numbers, positions, departments.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? EmployeeId = null,
    string? Position = null,
    string? DepartmentName = null,
    StaffType? StaffType = null,
    EmploymentStatus? EmploymentStatus = null,
    decimal? MinSalary = null,
    decimal? MaxSalary = null
) : IRequest<SalaryStatsDto>;

/// <summary>
/// Get salary statistics grouped by different parameters with filtering.
/// </summary>
public record GetGroupedSalaryStatsQuery(
    string? Keyword = null, // Search in employee names, staff numbers, positions, departments.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? EmployeeId = null,
    string? Position = null,
    string? DepartmentName = null,
    StaffType? StaffType = null,
    EmploymentStatus? EmploymentStatus = null,
    decimal? MinSalary = null,
    decimal? MaxSalary = null,
    string GroupBy = "Department", // Group by "Department", "Position", "StaffType", "Month", "Year".
    string? SortBy = "TotalSalaryPaid", // Sort by "TotalSalaryPaid", "AverageSalary", "EmployeeCount", "GroupName".
    bool Descending = true
) : IRequest<List<GroupedSalaryStatsDto>>;

/// <summary>
/// Get salary records for a specific employee.
/// </summary>
public record GetEmployeeSalaryRecordsQuery(
    int EmployeeId,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? SortBy = "PayDate",
    bool Descending = true,
    int Page = 1,
    int PageSize = 20
) : IRequest<SalaryRecordResult>;

/// <summary>
/// Get salary summary for a specific employee.
/// </summary>
public record GetEmployeeSalarySummaryQuery(
    int EmployeeId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<EmployeeSalarySummaryDto?>;

/// <summary>
/// Get monthly salary report.
/// </summary>
public record GetMonthlySalaryReportQuery(
    int? Year = null,
    int? Month = null,
    string? DepartmentName = null,
    StaffType? StaffType = null,
    string? SortBy = "Year,Month", // Sort by "Year,Month", "TotalSalaryPaid", "EmployeesPaid".
    bool Descending = true
) : IRequest<List<MonthlySalaryReportDto>>;

/// <summary>
/// Get salary records by date range for payroll processing.
/// </summary>
public record GetPayrollQuery(
    DateTime PayDate,
    string? DepartmentName = null,
    StaffType? StaffType = null,
    EmploymentStatus? EmploymentStatus = null
) : IRequest<List<SalaryRecordSummaryDto>>;
