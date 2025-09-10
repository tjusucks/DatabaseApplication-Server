using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.Attendances;

public class CreateAttendanceCommandHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<CreateAttendanceCommand, int>
{
    private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;

    public async Task<int> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
    {
        // 检查是否已存在该日期的考勤记录
        var existing = await _attendanceRepository.GetByEmployeeAndDateAsync(
            request.EmployeeId, request.AttendanceDate);

        if (existing != null)
        {
            throw new InvalidOperationException(
                $"该员工在 {request.AttendanceDate:yyyy-MM-dd} 已有考勤记录");
        }
        var checkInTime = request.CheckInTime ?? request.AttendanceDate.Date;
        var attendance = new Attendance
        {
            EmployeeId = request.EmployeeId,
            AttendanceDate = request.AttendanceDate,
            CheckInTime = checkInTime,
            CheckOutTime = request.CheckOutTime,
            AttendanceStatus = request.Status,
            LeaveType = request.LeaveType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _attendanceRepository.AddAsync(attendance);
        return attendance.AttendanceId;
    }
}

public class UpdateAttendanceCommandHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<UpdateAttendanceCommand>
{
    private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;

    public async Task Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
    {
        var attendance = await _attendanceRepository.GetByIdAsync(request.AttendanceId)
            ?? throw new KeyNotFoundException($"考勤记录 ID {request.AttendanceId} 不存在");

        // 更新考勤记录
        if (request.CheckInTime.HasValue) attendance.CheckInTime = (DateTime)request.CheckInTime;
        if (request.CheckOutTime.HasValue) attendance.CheckOutTime = request.CheckOutTime;
        attendance.AttendanceStatus = request.Status;
        attendance.LeaveType = request.LeaveType;
        attendance.UpdatedAt = DateTime.UtcNow;

        await _attendanceRepository.UpdateAsync(attendance);
    }
}

public class DeleteAttendanceCommandHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<DeleteAttendanceCommand>
{
    private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;

    public async Task Handle(DeleteAttendanceCommand request, CancellationToken cancellationToken)
    {
        await _attendanceRepository.DeleteAsync(request.AttendanceId);
    }
}

public class RecordCheckInCommandHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<RecordCheckInCommand>
{
    private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;
    private readonly TimeSpan _lateThreshold = TimeSpan.FromHours(9);

    public async Task Handle(RecordCheckInCommand request, CancellationToken cancellationToken)
    {
        var date = request.CheckInTime.Date;
        var existing = await _attendanceRepository.GetByEmployeeAndDateAsync(request.EmployeeId, date);

        if (existing != null)
        {
            existing.CheckInTime = request.CheckInTime;
            existing.AttendanceStatus = DetermineStatus(request.CheckInTime);
            existing.UpdatedAt = DateTime.UtcNow;
            await _attendanceRepository.UpdateAsync(existing);
            return;
        }

        var attendance = new Attendance
        {
            EmployeeId = request.EmployeeId,
            AttendanceDate = date,
            CheckInTime = request.CheckInTime,
            AttendanceStatus = DetermineStatus(request.CheckInTime),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _attendanceRepository.AddAsync(attendance);
    }

    private AttendanceStatus DetermineStatus(DateTime checkInTime)
    {
        var workStart = checkInTime.Date.Add(_lateThreshold);
        return checkInTime > workStart ? AttendanceStatus.Late : AttendanceStatus.Present;
    }
}

public class RecordCheckOutCommandHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<RecordCheckOutCommand>
{
    private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;

    public async Task Handle(RecordCheckOutCommand request, CancellationToken cancellationToken)
    {
        var date = request.CheckOutTime.Date;
        var attendance = await _attendanceRepository.GetByEmployeeAndDateAsync(request.EmployeeId, date);

        if (attendance == null)
        {
            attendance = new Attendance
            {
                EmployeeId = request.EmployeeId,
                AttendanceDate = date,
                CheckOutTime = request.CheckOutTime,
                AttendanceStatus = AttendanceStatus.Absent,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _attendanceRepository.AddAsync(attendance);
        }
        else
        {
            attendance.CheckOutTime = request.CheckOutTime;
            attendance.UpdatedAt = DateTime.UtcNow;
            await _attendanceRepository.UpdateAsync(attendance);
        }
    }
}

public class ApplyLeaveCommandHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<ApplyLeaveCommand>
{
    private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;

    public async Task Handle(ApplyLeaveCommand request, CancellationToken cancellationToken)
    {
        var attendance = await _attendanceRepository.GetByEmployeeAndDateAsync(request.EmployeeId, request.Date);

        if (attendance == null)
        {
            attendance = new Attendance
            {
                EmployeeId = request.EmployeeId,
                AttendanceDate = request.Date,
                AttendanceStatus = AttendanceStatus.Leave,
                LeaveType = request.LeaveType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _attendanceRepository.AddAsync(attendance);
        }
        else
        {
            attendance.AttendanceStatus = AttendanceStatus.Leave;
            attendance.LeaveType = request.LeaveType;
            attendance.UpdatedAt = DateTime.UtcNow;
            await _attendanceRepository.UpdateAsync(attendance);
        }
    }
}
