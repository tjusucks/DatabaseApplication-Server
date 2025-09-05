using System;
using System.Collections.Generic;
using DbApp.Domain.Entities.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.Attendances
{
    // 获取考勤记录查询
    public record GetAttendanceByIdQuery(int Id) : IRequest<Attendance?>;
    public record GetEmployeeAttendanceQuery(int EmployeeId, DateTime? StartDate, DateTime? EndDate) : IRequest<List<Attendance>>;
    public record GetDepartmentAttendanceQuery(string DepartmentId, DateTime? StartDate, DateTime? EndDate) : IRequest<List<Attendance>>;

    // 获取异常考勤记录查询
    public record GetAbnormalRecordsQuery(int? EmployeeId, DateTime StartDate, DateTime EndDate) : IRequest<List<Attendance>>;

    // 获取统计信息查询
    public record GetEmployeeStatsQuery(int EmployeeId, DateTime? StartDate, DateTime? EndDate) : IRequest<EmployeeStatsResponse>;
    public record GetEmployeeMonthlyStatsQuery(int EmployeeId, int Year, int Month) : IRequest<MonthlyStatsResponse>;
    public record GetDepartmentStatsQuery(string DepartmentId, DateTime? StartDate, DateTime? EndDate) : IRequest<DepartmentStatsResponse>;

    // 获取员工是否全勤查询
    public record CheckEmployeeFullAttendanceQuery(int EmployeeId, int Year, int Month) : IRequest<bool>;
    // 响应模型
    public class EmployeeStatsResponse
    {
        public int PresentDays { get; set; }
        public int LateDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public int TotalWorkingDays { get; set; }
        public decimal AttendanceRate { get; set; }
    }

    public class MonthlyStatsResponse
    {
        public int PresentDays { get; set; }
        public int LateDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public bool IsFullAttendance { get; set; }
    }

    public class DepartmentStatsResponse
    {
        public int TotalEmployees { get; set; }
        public int PresentDays { get; set; }
        public int LateDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public decimal OverallAttendanceRate { get; set; }
    }
    //统一查询模型
    public record GenericAttendanceQueryRequest(
        AttendanceQueryType QueryType, // 指定查询类型
        int? Id = null,
        int? EmployeeId = null,
        string? DepartmentId = null,
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        int? Year = null,
        int? Month = null
    ) : IRequest<object>;
    //查询类型枚举
    public enum AttendanceQueryType
    {
        GetById,
        GetEmployeeAttendance,
        GetDepartmentAttendance,
        GetAbnormalRecords,
        GetEmployeeStats,
        GetEmployeeMonthlyStats,
        GetDepartmentStats,
        CheckFullAttendance
    }
}
