using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.ResourceSystem.Attendances
{
    public class GetAttendanceByIdQueryHandler : IRequestHandler<GetAttendanceByIdQuery, Attendance?>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public GetAttendanceByIdQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<Attendance?> Handle(GetAttendanceByIdQuery request, CancellationToken cancellationToken)
        {
            return await _attendanceRepository.GetByIdAsync(request.Id);
        }
    }

    public class GetEmployeeAttendanceQueryHandler : IRequestHandler<GetEmployeeAttendanceQuery, List<Attendance>>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public GetEmployeeAttendanceQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<List<Attendance>> Handle(GetEmployeeAttendanceQuery request, CancellationToken cancellationToken)
        {
            return (await _attendanceRepository.GetByEmployeeAsync(
                request.EmployeeId, 
                request.StartDate, 
                request.EndDate
            )).ToList();
        }
    }

    public class GetDepartmentAttendanceQueryHandler : IRequestHandler<GetDepartmentAttendanceQuery, List<Attendance>>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public GetDepartmentAttendanceQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<List<Attendance>> Handle(GetDepartmentAttendanceQuery request, CancellationToken cancellationToken)
        {
            return (await _attendanceRepository.GetByDepartmentAsync(
                request.DepartmentId, 
                request.StartDate, 
                request.EndDate
            )).ToList();
        }
    }

    public class GetAbnormalRecordsQueryHandler : IRequestHandler<GetAbnormalRecordsQuery, List<Attendance>>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public GetAbnormalRecordsQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<List<Attendance>> Handle(GetAbnormalRecordsQuery request, CancellationToken cancellationToken)
        {
            return (await _attendanceRepository.GetAbnormalRecordsAsync(
                request.EmployeeId, 
                request.StartDate, 
                request.EndDate
            )).ToList();
        }
    }

    public class GetEmployeeStatsQueryHandler : IRequestHandler<GetEmployeeStatsQuery, EmployeeStatsResponse>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public GetEmployeeStatsQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<EmployeeStatsResponse> Handle(GetEmployeeStatsQuery request, CancellationToken cancellationToken)
        {
            var (presentDays, lateDays, absentDays, leaveDays) = 
                await _attendanceRepository.GetEmployeeStatsAsync(
                    request.EmployeeId, 
                    request.StartDate, 
                    request.EndDate
                );
            
            var totalDays = presentDays + lateDays + absentDays + leaveDays;
            var attendanceRate = totalDays > 0 
                ? (decimal)(presentDays + leaveDays) / totalDays * 100 
                : 0;
            
            return new EmployeeStatsResponse
            {
                PresentDays = presentDays,
                LateDays = lateDays,
                AbsentDays = absentDays,
                LeaveDays = leaveDays,
                TotalWorkingDays = totalDays,
                AttendanceRate = attendanceRate
            };
        }
    }

    public class GetEmployeeMonthlyStatsQueryHandler : IRequestHandler<GetEmployeeMonthlyStatsQuery, MonthlyStatsResponse>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public GetEmployeeMonthlyStatsQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<MonthlyStatsResponse> Handle(GetEmployeeMonthlyStatsQuery request, CancellationToken cancellationToken)
        {
            var (presentDays, lateDays, absentDays, leaveDays) = 
                await _attendanceRepository.GetEmployeeStatsAsync(
                    request.EmployeeId, 
                    new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Local), 
                    new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Local).AddMonths(1).AddDays(-1)
                );
            
            return new MonthlyStatsResponse
            {
                PresentDays = presentDays,
                LateDays = lateDays,
                AbsentDays = absentDays,
                LeaveDays = leaveDays,
                IsFullAttendance = lateDays == 0 && absentDays == 0
            };
        }
    }
    public class UpdateAttendanceStatusCommandHandler : IRequestHandler<UpdateAttendanceStatusCommand>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public UpdateAttendanceStatusCommandHandler(IAttendanceRepository attendanceRepository)
            => _attendanceRepository = attendanceRepository;

        public async Task Handle(UpdateAttendanceStatusCommand request, CancellationToken cancellationToken)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(request.AttendanceId);
            if (attendance == null) throw new KeyNotFoundException("考勤记录不存在");
        
            attendance.AttendanceStatus = request.Status;
            await _attendanceRepository.UpdateAsync(attendance);
        }
    }
    public class GetDepartmentStatsQueryHandler : IRequestHandler<GetDepartmentStatsQuery, DepartmentStatsResponse>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public GetDepartmentStatsQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<DepartmentStatsResponse> Handle(GetDepartmentStatsQuery request, CancellationToken cancellationToken)
        {
            var departmentAttendances = await _attendanceRepository.GetByDepartmentAsync(
                request.DepartmentId, 
                request.StartDate, 
                request.EndDate
            );
            
            var employeeGroups = departmentAttendances.GroupBy(a => a.EmployeeId);
            var totalEmployees = employeeGroups.Count();
            
            int presentDays = 0, lateDays = 0, absentDays = 0, leaveDays = 0;
            
           var stats = departmentAttendances
           .GroupBy(a => 1) // 全局分组
           .Select(g => new {
           PresentDays = g.Count(a => a.AttendanceStatus == AttendanceStatus.Present),
           LateDays = g.Count(a => a.AttendanceStatus == AttendanceStatus.Late),
           AbsentDays = g.Count(a => a.AttendanceStatus == AttendanceStatus.Absent),
           LeaveDays = g.Count(a => a.AttendanceStatus == AttendanceStatus.Leave)
           }).FirstOrDefault();
            // 判空赋值
            if (stats != null)
            {
                presentDays = stats.PresentDays;
                lateDays = stats.LateDays;
                absentDays = stats.AbsentDays;
                leaveDays = stats.LeaveDays;
            }
            int totalDays = presentDays + lateDays + absentDays + leaveDays;
            var attendanceRate = totalDays > 0 
                ? (decimal)(presentDays + leaveDays) / totalDays * 100 
                : 0;
            
            return new DepartmentStatsResponse
            {
                TotalEmployees = totalEmployees,
                PresentDays = presentDays,
                LateDays = lateDays,
                AbsentDays = absentDays,
                LeaveDays = leaveDays,
                OverallAttendanceRate = attendanceRate
            };
        }
    }
}