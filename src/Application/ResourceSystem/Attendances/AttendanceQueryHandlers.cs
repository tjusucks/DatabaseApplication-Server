using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

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
            .Select(g => new
            {
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
    public class CheckEmployeeFullAttendanceQueryHandler : IRequestHandler<CheckEmployeeFullAttendanceQuery, bool>
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public CheckEmployeeFullAttendanceQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<bool> Handle(CheckEmployeeFullAttendanceQuery request, CancellationToken cancellationToken)
        {
            var startDate = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Local);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // 只获取需要的统计项（避免未使用变量警告）
            var (_, lateDays, absentDays, _) =
                await _attendanceRepository.GetEmployeeStatsAsync(
                    request.EmployeeId,
                    startDate,
                    endDate
                );

            return lateDays == 0 && absentDays == 0;
        }
    }

    //统一查询处理器
    public class GenericAttendanceQueryHandler : IRequestHandler<GenericAttendanceQueryRequest, object>
    {
        private readonly IMediator _mediator;

        public GenericAttendanceQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<object> Handle(GenericAttendanceQueryRequest request, CancellationToken cancellationToken)
        {
            return request.QueryType switch
            {
                AttendanceQueryType.GetById => await HandleGetById(request),
                AttendanceQueryType.GetEmployeeAttendance => await HandleEmployeeAttendance(request),
                AttendanceQueryType.GetDepartmentAttendance => await HandleDepartmentAttendance(request),
                AttendanceQueryType.GetAbnormalRecords => await HandleAbnormalRecords(request),
                AttendanceQueryType.GetEmployeeStats => await HandleEmployeeStats(request),
                AttendanceQueryType.GetEmployeeMonthlyStats => await HandleEmployeeMonthlyStats(request),
                AttendanceQueryType.GetDepartmentStats => await HandleDepartmentStats(request),
                AttendanceQueryType.CheckFullAttendance => await HandleCheckFullAttendance(request),
                _ => throw new ArgumentException($"未知查询类型: {request.QueryType}")
            };
        }

        private async Task<object> HandleGetById(GenericAttendanceQueryRequest request)
        {
            if (!request.Id.HasValue) throw new ArgumentException("缺少ID参数");
            var result = await _mediator.Send(new GetAttendanceByIdQuery(request.Id.Value));
            return result ?? throw new KeyNotFoundException($"找不到ID为{request.Id.Value}的考勤记录");
        }

        private async Task<object> HandleEmployeeAttendance(GenericAttendanceQueryRequest request)
        {
            if (!request.EmployeeId.HasValue) throw new ArgumentException("缺少EmployeeId参数");
            return await _mediator.Send(new GetEmployeeAttendanceQuery(
                request.EmployeeId.Value,
                request.StartDate,
                request.EndDate));
        }

        private async Task<object> HandleDepartmentAttendance(GenericAttendanceQueryRequest request)
        {
            if (string.IsNullOrEmpty(request.DepartmentId)) throw new ArgumentException("缺少DepartmentId参数");
            return await _mediator.Send(new GetDepartmentAttendanceQuery(
                request.DepartmentId,
                request.StartDate,
                request.EndDate));
        }

        private async Task<object> HandleAbnormalRecords(GenericAttendanceQueryRequest request)
        {
            if (!request.StartDate.HasValue || !request.EndDate.HasValue)
                throw new ArgumentException("缺少时间范围参数");

            return await _mediator.Send(new GetAbnormalRecordsQuery(
                request.EmployeeId,
                request.StartDate.Value,
                request.EndDate.Value));
        }

        private async Task<object> HandleEmployeeStats(GenericAttendanceQueryRequest request)
        {
            if (!request.EmployeeId.HasValue) throw new ArgumentException("缺少EmployeeId参数");
            return await _mediator.Send(new GetEmployeeStatsQuery(
                request.EmployeeId.Value,
                request.StartDate,
                request.EndDate));
        }

        private async Task<object> HandleEmployeeMonthlyStats(GenericAttendanceQueryRequest request)
        {
            if (!request.EmployeeId.HasValue || !request.Year.HasValue || !request.Month.HasValue)
                throw new ArgumentException("缺少EmployeeId、Year或Month参数");

            return await _mediator.Send(new GetEmployeeMonthlyStatsQuery(
                request.EmployeeId.Value,
                request.Year.Value,
                request.Month.Value));
        }

        private async Task<object> HandleDepartmentStats(GenericAttendanceQueryRequest request)
        {
            if (string.IsNullOrEmpty(request.DepartmentId)) throw new ArgumentException("缺少DepartmentId参数");
            return await _mediator.Send(new GetDepartmentStatsQuery(
                request.DepartmentId,
                request.StartDate,
                request.EndDate));
        }

        private async Task<object> HandleCheckFullAttendance(GenericAttendanceQueryRequest request)
        {
            if (!request.EmployeeId.HasValue || !request.Year.HasValue || !request.Month.HasValue)
                throw new ArgumentException("缺少EmployeeId、Year或Month参数");

            return await _mediator.Send(new CheckEmployeeFullAttendanceQuery(
                request.EmployeeId.Value,
                request.Year.Value,
                request.Month.Value));
        }
    }
}
