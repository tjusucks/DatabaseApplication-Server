using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Microsoft.EntityFrameworkCore;
using DbApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbApp.Infrastructure.Repositories.ResourceSystem
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsync(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Attendance?> GetByIdAsync(int id)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
        }

        public async Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateTime date)
        {
            return await _context.Attendances
                .Where(a => a.EmployeeId == employeeId && a.AttendanceDate.Date == date.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Attendance>> GetByEmployeeAsync(int employeeId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Attendances
                .Where(a => a.EmployeeId == employeeId)
                .OrderBy(a => a.AttendanceDate);

            if (startDate.HasValue)
                query = (IOrderedQueryable<Attendance>)query.Where(a => a.AttendanceDate >= startDate.Value);

            if (endDate.HasValue)
                query = (IOrderedQueryable<Attendance>)query.Where(a => a.AttendanceDate <= endDate.Value);

            return await query.ToListAsync();
        }
        public async Task<(int presentDays, int lateDays, int absentDays, int leaveDays)> GetDepartmentStatsAsync(string departmentId, DateTime? startDate, DateTime? endDate)
        {
            var records = await GetByDepartmentAsync(departmentId, startDate, endDate);
            return (
            records.Count(a => a.AttendanceStatus == AttendanceStatus.Present),
            records.Count(a => a.AttendanceStatus == AttendanceStatus.Late),
            records.Count(a => a.AttendanceStatus == AttendanceStatus.Absent),
            records.Count(a => a.AttendanceStatus == AttendanceStatus.Leave)
           );
        }
        public async Task<IEnumerable<Attendance>> GetByDepartmentAsync(string departmentId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Employee.DepartmentName == departmentId)
                .OrderBy(a => a.AttendanceDate);

            if (startDate.HasValue)
                query = (IOrderedQueryable<Attendance>)query.Where(a => a.AttendanceDate >= startDate.Value);

            if (endDate.HasValue)
                query = (IOrderedQueryable<Attendance>)query.Where(a => a.AttendanceDate <= endDate.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetAbnormalRecordsAsync(int? employeeId, DateTime startDate, DateTime endDate)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.AttendanceDate >= startDate && a.AttendanceDate <= endDate)
                .Where(a => a.AttendanceStatus != AttendanceStatus.Present);

            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId.Value);

            return await query.OrderBy(a => a.AttendanceDate).ToListAsync();
        }

        public async Task<(int presentDays, int lateDays, int absentDays, int leaveDays)> GetEmployeeStatsAsync(
            int employeeId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Attendances
                .Where(a => a.EmployeeId == employeeId);

            if (startDate.HasValue)
                query = query.Where(a => a.AttendanceDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.AttendanceDate <= endDate.Value);

            var result = await query
                .GroupBy(a => 1)
                .Select(g => new
                {
                    PresentDays = g.Count(a => a.AttendanceStatus == AttendanceStatus.Present),
                    LateDays = g.Count(a => a.AttendanceStatus == AttendanceStatus.Late),
                    AbsentDays = g.Count(a => a.AttendanceStatus == AttendanceStatus.Absent),
                    LeaveDays = g.Count(a => a.AttendanceStatus == AttendanceStatus.Leave)
                })
                .FirstOrDefaultAsync();

            return (
                result?.PresentDays ?? 0,
                result?.LateDays ?? 0,
                result?.AbsentDays ?? 0,
                result?.LeaveDays ?? 0
            );
        }

        public async Task<decimal> GetEmployeeAttendanceRateAsync(
            int employeeId, DateTime? startDate, DateTime? endDate)
        {
            var (presentDays, lateDays, absentDays, leaveDays) = 
                await GetEmployeeStatsAsync(employeeId, startDate, endDate);
            
            int totalDays = presentDays + lateDays + absentDays + leaveDays;
            
            return totalDays > 0 
                ? (decimal)(presentDays + leaveDays) / totalDays * 100 
                : 0;
        }

        public async Task<bool> IsEmployeeFullAttendanceAsync(int employeeId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Local);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            
            var (_, lateDays, absentDays, _) = 
                await GetEmployeeStatsAsync(employeeId, startDate, endDate);
            
            return lateDays == 0 && absentDays == 0;
        }
    }
}