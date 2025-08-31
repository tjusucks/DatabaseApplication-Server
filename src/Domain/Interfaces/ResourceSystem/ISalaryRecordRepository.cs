using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Statistics.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface ISalaryRecordRepository
{
    Task<List<SalaryRecord>> SearchAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        int? employeeId,
        string? position,
        string? departmentName,
        StaffType? staffType,
        EmploymentStatus? employmentStatus,
        decimal? minSalary,
        decimal? maxSalary,
        string? sortBy,
        bool descending,
        int page,
        int pageSize);

    Task<int> CountAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        int? employeeId,
        string? position,
        string? departmentName,
        StaffType? staffType,
        EmploymentStatus? employmentStatus,
        decimal? minSalary,
        decimal? maxSalary);

    Task<SalaryRecord?> GetByIdAsync(int salaryRecordId);

    Task<SalaryStats> GetStatsAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        int? employeeId,
        string? position,
        string? departmentName,
        StaffType? staffType,
        EmploymentStatus? employmentStatus,
        decimal? minSalary,
        decimal? maxSalary);

    Task<List<GroupedSalaryStats>> GetGroupedStatsAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        int? employeeId,
        string? position,
        string? departmentName,
        StaffType? staffType,
        EmploymentStatus? employmentStatus,
        decimal? minSalary,
        decimal? maxSalary,
        string groupBy,
        string? sortBy,
        bool descending);

    Task<List<SalaryRecord>> GetByEmployeeAsync(
        int employeeId,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,
        bool descending,
        int page,
        int pageSize);

    Task<int> CountByEmployeeAsync(
        int employeeId,
        DateTime? startDate,
        DateTime? endDate);

    Task<EmployeeSalarySummary?> GetEmployeeSalarySummaryAsync(
        int employeeId,
        DateTime? startDate,
        DateTime? endDate);

    Task<List<MonthlySalaryReport>> GetMonthlySalaryReportAsync(
        int? year,
        int? month,
        string? departmentName,
        StaffType? staffType,
        string? sortBy,
        bool descending);

    Task<List<SalaryRecord>> GetPayrollAsync(
        DateTime payDate,
        string? departmentName,
        StaffType? staffType,
        EmploymentStatus? employmentStatus);

    Task<SalaryRecord> AddAsync(SalaryRecord salaryRecord);
    Task<List<SalaryRecord>> AddBatchAsync(List<SalaryRecord> salaryRecords);
    Task<SalaryRecord> UpdateAsync(SalaryRecord salaryRecord);
    Task<bool> DeleteAsync(int salaryRecordId);
}
