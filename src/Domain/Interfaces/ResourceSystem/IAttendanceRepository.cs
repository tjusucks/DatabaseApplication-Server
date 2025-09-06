using DbApp.Domain.Entities.ResourceSystem;
namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IAttendanceRepository
{
    // 基础CRUD操作
    Task AddAsync(Attendance attendance);
    Task UpdateAsync(Attendance attendance);
    Task DeleteAsync(int id);
    Task<Attendance?> GetByIdAsync(int id);

    // 查询操作
    Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateTime date);
    Task<IEnumerable<Attendance>> GetByEmployeeAsync(int employeeId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<Attendance>> GetByDepartmentAsync(string departmentId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<Attendance>> GetAbnormalRecordsAsync(int? employeeId, DateTime startDate, DateTime endDate);

    // 统计方法
    Task<(int presentDays, int lateDays, int absentDays, int leaveDays)> GetEmployeeStatsAsync(
        int employeeId, DateTime? startDate, DateTime? endDate);

    Task<(int presentDays, int lateDays, int absentDays, int leaveDays)> GetDepartmentStatsAsync(
        string departmentId, DateTime? startDate, DateTime? endDate);

    Task<decimal> GetEmployeeAttendanceRateAsync(
        int employeeId, DateTime? startDate, DateTime? endDate);

    Task<bool> IsEmployeeFullAttendanceAsync(int employeeId, int year, int month);

}
