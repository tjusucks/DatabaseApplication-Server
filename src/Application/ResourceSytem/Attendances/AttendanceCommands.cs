using System;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.Attendances
{
    // 基础CRUD命令
    public record CreateAttendanceCommand(
        int EmployeeId,
        DateTime AttendanceDate,
        DateTime? CheckInTime,
        DateTime? CheckOutTime,
        AttendanceStatus Status,
        LeaveType? LeaveType) : IRequest<int>;

    public record UpdateAttendanceCommand(
        int AttendanceId,
        DateTime? CheckInTime,
        DateTime? CheckOutTime,
        AttendanceStatus Status,
        LeaveType? LeaveType) : IRequest;

    public record DeleteAttendanceCommand(int AttendanceId) : IRequest;

    // 业务操作命令
    public record RecordCheckInCommand(int EmployeeId, DateTime CheckInTime) : IRequest;
    public record RecordCheckOutCommand(int EmployeeId, DateTime CheckOutTime) : IRequest;
    public record ApplyLeaveCommand(int EmployeeId, DateTime Date, LeaveType LeaveType) : IRequest;
    public record UpdateAttendanceStatusCommand(int AttendanceId, AttendanceStatus Status) : IRequest;
}
