using System.Globalization;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

public class SalaryRecordRepository(ApplicationDbContext context) : ISalaryRecordRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<SalaryRecord>> SearchAsync(
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
        int pageSize)
    {
        var query = _context.SalaryRecords
            .Include(s => s.Employee)
                .ThenInclude(e => e.User)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(s =>
                s.Employee.User.DisplayName.Contains(keyword) ||
                s.Employee.StaffNumber.Contains(keyword) ||
                (s.Employee.Position != null && s.Employee.Position.Contains(keyword)) ||
                (s.Employee.DepartmentName != null && s.Employee.DepartmentName.Contains(keyword)) ||
                (s.Notes != null && s.Notes.Contains(keyword)));
        }

        if (startDate.HasValue)
        {
            query = query.Where(s => s.PayDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(s => s.PayDate <= endDate.Value);
        }

        if (employeeId.HasValue)
        {
            query = query.Where(s => s.EmployeeId == employeeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(position))
        {
            query = query.Where(s => s.Employee.Position != null && s.Employee.Position.Contains(position));
        }

        if (!string.IsNullOrWhiteSpace(departmentName))
        {
            query = query.Where(s => s.Employee.DepartmentName != null && s.Employee.DepartmentName.Contains(departmentName));
        }

        if (staffType.HasValue)
        {
            query = query.Where(s => s.Employee.StaffType == staffType.Value);
        }

        if (employmentStatus.HasValue)
        {
            query = query.Where(s => s.Employee.EmploymentStatus == employmentStatus.Value);
        }

        if (minSalary.HasValue)
        {
            query = query.Where(s => s.Salary >= minSalary.Value);
        }

        if (maxSalary.HasValue)
        {
            query = query.Where(s => s.Salary <= maxSalary.Value);
        }

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "salary" => descending ? query.OrderByDescending(s => s.Salary) : query.OrderBy(s => s.Salary),
            "employeename" => descending ? query.OrderByDescending(s => s.Employee.User.DisplayName) : query.OrderBy(s => s.Employee.User.DisplayName),
            "createdat" => descending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
            _ => descending ? query.OrderByDescending(s => s.PayDate) : query.OrderBy(s => s.PayDate)
        };

        // Apply pagination
        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        int? employeeId,
        string? position,
        string? departmentName,
        StaffType? staffType,
        EmploymentStatus? employmentStatus,
        decimal? minSalary,
        decimal? maxSalary)
    {
        var query = _context.SalaryRecords
            .Include(s => s.Employee)
                .ThenInclude(e => e.User)
            .AsQueryable();

        // Apply the same filters as SearchAsync
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(s =>
                s.Employee.User.DisplayName.Contains(keyword) ||
                s.Employee.StaffNumber.Contains(keyword) ||
                (s.Employee.Position != null && s.Employee.Position.Contains(keyword)) ||
                (s.Employee.DepartmentName != null && s.Employee.DepartmentName.Contains(keyword)) ||
                (s.Notes != null && s.Notes.Contains(keyword)));
        }

        if (startDate.HasValue)
            query = query.Where(s => s.PayDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.PayDate <= endDate.Value);

        if (employeeId.HasValue)
            query = query.Where(s => s.EmployeeId == employeeId.Value);

        if (!string.IsNullOrWhiteSpace(position))
            query = query.Where(s => s.Employee.Position != null && s.Employee.Position.Contains(position));

        if (!string.IsNullOrWhiteSpace(departmentName))
            query = query.Where(s => s.Employee.DepartmentName != null && s.Employee.DepartmentName.Contains(departmentName));

        if (staffType.HasValue)
            query = query.Where(s => s.Employee.StaffType == staffType.Value);

        if (employmentStatus.HasValue)
            query = query.Where(s => s.Employee.EmploymentStatus == employmentStatus.Value);

        if (minSalary.HasValue)
            query = query.Where(s => s.Salary >= minSalary.Value);

        if (maxSalary.HasValue)
            query = query.Where(s => s.Salary <= maxSalary.Value);

        return await query.CountAsync();
    }

    public async Task<SalaryRecord?> GetByIdAsync(int salaryRecordId)
    {
        return await _context.SalaryRecords
            .Include(s => s.Employee)
                .ThenInclude(e => e.User)
            .FirstOrDefaultAsync(s => s.SalaryRecordId == salaryRecordId);
    }

    public async Task<SalaryStats> GetStatsAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        int? employeeId,
        string? position,
        string? departmentName,
        StaffType? staffType,
        EmploymentStatus? employmentStatus,
        decimal? minSalary,
        decimal? maxSalary)
    {
        var query = _context.SalaryRecords
            .Include(s => s.Employee)
                .ThenInclude(e => e.User)
            .AsQueryable();

        // Apply the same filters
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(s =>
                s.Employee.User.DisplayName.Contains(keyword) ||
                s.Employee.StaffNumber.Contains(keyword) ||
                (s.Employee.Position != null && s.Employee.Position.Contains(keyword)) ||
                (s.Employee.DepartmentName != null && s.Employee.DepartmentName.Contains(keyword)) ||
                (s.Notes != null && s.Notes.Contains(keyword)));
        }

        if (startDate.HasValue)
            query = query.Where(s => s.PayDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(s => s.PayDate <= endDate.Value);
        if (employeeId.HasValue)
            query = query.Where(s => s.EmployeeId == employeeId.Value);
        if (!string.IsNullOrWhiteSpace(position))
            query = query.Where(s => s.Employee.Position != null && s.Employee.Position.Contains(position));
        if (!string.IsNullOrWhiteSpace(departmentName))
            query = query.Where(s => s.Employee.DepartmentName != null && s.Employee.DepartmentName.Contains(departmentName));
        if (staffType.HasValue)
            query = query.Where(s => s.Employee.StaffType == staffType.Value);
        if (employmentStatus.HasValue)
            query = query.Where(s => s.Employee.EmploymentStatus == employmentStatus.Value);
        if (minSalary.HasValue)
            query = query.Where(s => s.Salary >= minSalary.Value);
        if (maxSalary.HasValue)
            query = query.Where(s => s.Salary <= maxSalary.Value);

        var records = await query.ToListAsync();

        return new SalaryStats
        {
            TotalSalaryPaid = records.Sum(r => r.Salary),
            AverageSalary = records.Count > 0 ? records.Average(r => r.Salary) : 0,
            HighestSalary = records.Count > 0 ? records.Max(r => r.Salary) : 0,
            LowestSalary = records.Count > 0 ? records.Min(r => r.Salary) : 0,
            TotalEmployees = records.Select(r => r.EmployeeId).Distinct().Count(),
            TotalRecords = records.Count,
            FirstPayment = records.Count > 0 ? records.Min(r => r.PayDate) : null,
            LastPayment = records.Count > 0 ? records.Max(r => r.PayDate) : null
        };
    }

    public async Task<List<GroupedSalaryStats>> GetGroupedStatsAsync(
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
        bool descending)
    {
        var query = _context.SalaryRecords
            .Include(s => s.Employee)
                .ThenInclude(e => e.User)
            .AsQueryable();

        // Apply filters (same as before)
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(s =>
                s.Employee.User.DisplayName.Contains(keyword) ||
                s.Employee.StaffNumber.Contains(keyword) ||
                (s.Employee.Position != null && s.Employee.Position.Contains(keyword)) ||
                (s.Employee.DepartmentName != null && s.Employee.DepartmentName.Contains(keyword)) ||
                (s.Notes != null && s.Notes.Contains(keyword)));
        }

        if (startDate.HasValue)
            query = query.Where(s => s.PayDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(s => s.PayDate <= endDate.Value);
        if (employeeId.HasValue)
            query = query.Where(s => s.EmployeeId == employeeId.Value);
        if (!string.IsNullOrWhiteSpace(position))
            query = query.Where(s => s.Employee.Position != null && s.Employee.Position.Contains(position));
        if (!string.IsNullOrWhiteSpace(departmentName))
            query = query.Where(s => s.Employee.DepartmentName != null && s.Employee.DepartmentName.Contains(departmentName));
        if (staffType.HasValue)
            query = query.Where(s => s.Employee.StaffType == staffType.Value);
        if (employmentStatus.HasValue)
            query = query.Where(s => s.Employee.EmploymentStatus == employmentStatus.Value);
        if (minSalary.HasValue)
            query = query.Where(s => s.Salary >= minSalary.Value);
        if (maxSalary.HasValue)
            query = query.Where(s => s.Salary <= maxSalary.Value);

        var records = await query.ToListAsync();

        var groupedData = groupBy.ToLower() switch
        {
            "department" => records.Where(r => r.Employee.DepartmentName != null)
                .GroupBy(r => new { Key = r.Employee.DepartmentName, Name = r.Employee.DepartmentName })
                .Select(g => new GroupedSalaryStats
                {
                    GroupKey = g.Key.Key!,
                    GroupName = g.Key.Name!,
                    TotalSalaryPaid = g.Sum(r => r.Salary),
                    AverageSalary = g.Average(r => r.Salary),
                    HighestSalary = g.Max(r => r.Salary),
                    LowestSalary = g.Min(r => r.Salary),
                    EmployeeCount = g.Select(r => r.EmployeeId).Distinct().Count(),
                    RecordCount = g.Count(),
                    FirstPayment = g.Any() ? g.Min(r => r.PayDate) : null,
                    LastPayment = g.Any() ? g.Max(r => r.PayDate) : null
                }),
            "position" => records.Where(r => r.Employee.Position != null)
                .GroupBy(r => new { Key = r.Employee.Position, Name = r.Employee.Position })
                .Select(g => new GroupedSalaryStats
                {
                    GroupKey = g.Key.Key!,
                    GroupName = g.Key.Name!,
                    TotalSalaryPaid = g.Sum(r => r.Salary),
                    AverageSalary = g.Average(r => r.Salary),
                    HighestSalary = g.Max(r => r.Salary),
                    LowestSalary = g.Min(r => r.Salary),
                    EmployeeCount = g.Select(r => r.EmployeeId).Distinct().Count(),
                    RecordCount = g.Count(),
                    FirstPayment = g.Any() ? g.Min(r => r.PayDate) : null,
                    LastPayment = g.Any() ? g.Max(r => r.PayDate) : null
                }),
            "stafftype" => records.Where(r => r.Employee.StaffType != null)
                .GroupBy(r => new { Key = r.Employee.StaffType.ToString() ?? "Unknown", Name = r.Employee.StaffType.ToString() ?? "Unknown" })
                .Select(g => new GroupedSalaryStats
                {
                    GroupKey = g.Key.Key,
                    GroupName = g.Key.Name,
                    TotalSalaryPaid = g.Sum(r => r.Salary),
                    AverageSalary = g.Average(r => r.Salary),
                    HighestSalary = g.Max(r => r.Salary),
                    LowestSalary = g.Min(r => r.Salary),
                    EmployeeCount = g.Select(r => r.EmployeeId).Distinct().Count(),
                    RecordCount = g.Count(),
                    FirstPayment = g.Any() ? g.Min(r => r.PayDate) : null,
                    LastPayment = g.Any() ? g.Max(r => r.PayDate) : null
                }),
            "month" => records.GroupBy(r => new { Key = r.PayDate.ToString("yyyy-MM"), Name = r.PayDate.ToString("yyyy年MM月") })
                .Select(g => new GroupedSalaryStats
                {
                    GroupKey = g.Key.Key,
                    GroupName = g.Key.Name,
                    TotalSalaryPaid = g.Sum(r => r.Salary),
                    AverageSalary = g.Average(r => r.Salary),
                    HighestSalary = g.Max(r => r.Salary),
                    LowestSalary = g.Min(r => r.Salary),
                    EmployeeCount = g.Select(r => r.EmployeeId).Distinct().Count(),
                    RecordCount = g.Count(),
                    FirstPayment = g.Any() ? g.Min(r => r.PayDate) : null,
                    LastPayment = g.Any() ? g.Max(r => r.PayDate) : null
                }),
            "year" => records.GroupBy(r => new { Key = r.PayDate.Year.ToString(), Name = r.PayDate.Year.ToString() + "年" })
                .Select(g => new GroupedSalaryStats
                {
                    GroupKey = g.Key.Key,
                    GroupName = g.Key.Name,
                    TotalSalaryPaid = g.Sum(r => r.Salary),
                    AverageSalary = g.Average(r => r.Salary),
                    HighestSalary = g.Max(r => r.Salary),
                    LowestSalary = g.Min(r => r.Salary),
                    EmployeeCount = g.Select(r => r.EmployeeId).Distinct().Count(),
                    RecordCount = g.Count(),
                    FirstPayment = g.Any() ? g.Min(r => r.PayDate) : null,
                    LastPayment = g.Any() ? g.Max(r => r.PayDate) : null
                }),
            _ => records.Where(r => r.Employee.DepartmentName != null)
                .GroupBy(r => new { Key = r.Employee.DepartmentName, Name = r.Employee.DepartmentName })
                .Select(g => new GroupedSalaryStats
                {
                    GroupKey = g.Key.Key!,
                    GroupName = g.Key.Name!,
                    TotalSalaryPaid = g.Sum(r => r.Salary),
                    AverageSalary = g.Average(r => r.Salary),
                    HighestSalary = g.Max(r => r.Salary),
                    LowestSalary = g.Min(r => r.Salary),
                    EmployeeCount = g.Select(r => r.EmployeeId).Distinct().Count(),
                    RecordCount = g.Count(),
                    FirstPayment = g.Any() ? g.Min(r => r.PayDate) : null,
                    LastPayment = g.Any() ? g.Max(r => r.PayDate) : null
                })
        };

        var result = groupedData.ToList();

        // Apply sorting
        result = sortBy?.ToLower() switch
        {
            "averagesalary" => descending ? [.. result.OrderByDescending(g => g.AverageSalary)] : [.. result.OrderBy(g => g.AverageSalary)],
            "employeecount" => descending ? [.. result.OrderByDescending(g => g.EmployeeCount)] : [.. result.OrderBy(g => g.EmployeeCount)],
            "groupname" => descending ? [.. result.OrderByDescending(g => g.GroupName)] : [.. result.OrderBy(g => g.GroupName)],
            _ => descending ? [.. result.OrderByDescending(g => g.TotalSalaryPaid)] : [.. result.OrderBy(g => g.TotalSalaryPaid)]
        };

        return result;
    }

    public async Task<List<SalaryRecord>> GetByEmployeeAsync(
        int employeeId,
        DateTime? startDate,
        DateTime? endDate,
        string? sortBy,
        bool descending,
        int page,
        int pageSize)
    {
        var query = _context.SalaryRecords
            .Include(s => s.Employee)
                .ThenInclude(e => e.User)
            .Where(s => s.EmployeeId == employeeId);

        if (startDate.HasValue)
            query = query.Where(s => s.PayDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.PayDate <= endDate.Value);

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "salary" => descending ? query.OrderByDescending(s => s.Salary) : query.OrderBy(s => s.Salary),
            "createdat" => descending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
            _ => descending ? query.OrderByDescending(s => s.PayDate) : query.OrderBy(s => s.PayDate)
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByEmployeeAsync(
        int employeeId,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _context.SalaryRecords
            .Where(s => s.EmployeeId == employeeId);

        if (startDate.HasValue)
            query = query.Where(s => s.PayDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.PayDate <= endDate.Value);

        return await query.CountAsync();
    }

    public async Task<EmployeeSalarySummary?> GetEmployeeSalarySummaryAsync(
        int employeeId,
        DateTime? startDate,
        DateTime? endDate)
    {
        var employee = await _context.Employees
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

        if (employee == null)
            return null;

        var query = _context.SalaryRecords
            .Where(s => s.EmployeeId == employeeId);

        if (startDate.HasValue)
            query = query.Where(s => s.PayDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.PayDate <= endDate.Value);

        var records = await query.ToListAsync();

        if (records.Count == 0)
            return null;

        var latestRecord = records.OrderByDescending(r => r.PayDate).First();

        return new EmployeeSalarySummary
        {
            EmployeeId = employeeId,
            EmployeeName = employee.User.DisplayName,
            StaffNumber = employee.StaffNumber,
            Position = employee.Position,
            DepartmentName = employee.DepartmentName,
            TotalSalaryReceived = records.Sum(r => r.Salary),
            AverageSalary = records.Average(r => r.Salary),
            LatestSalary = latestRecord.Salary,
            LatestPayDate = latestRecord.PayDate,
            TotalPayments = records.Count,
            FirstPayment = records.Min(r => r.PayDate),
            LastPayment = records.Max(r => r.PayDate)
        };
    }

    public async Task<List<MonthlySalaryReport>> GetMonthlySalaryReportAsync(
        int? year,
        int? month,
        string? departmentName,
        StaffType? staffType,
        string? sortBy,
        bool descending)
    {
        var query = _context.SalaryRecords
            .Include(s => s.Employee)
            .AsQueryable();

        if (year.HasValue)
            query = query.Where(s => s.PayDate.Year == year.Value);

        if (month.HasValue)
            query = query.Where(s => s.PayDate.Month == month.Value);

        if (!string.IsNullOrWhiteSpace(departmentName))
            query = query.Where(s => s.Employee.DepartmentName != null && s.Employee.DepartmentName.Contains(departmentName));

        if (staffType.HasValue)
            query = query.Where(s => s.Employee.StaffType == staffType.Value);

        var records = await query.ToListAsync();

        var groupedByMonth = records
            .GroupBy(r => new { r.PayDate.Year, r.PayDate.Month })
            .Select(g => new MonthlySalaryReport
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(g.Key.Month),
                TotalSalaryPaid = g.Sum(r => r.Salary),
                EmployeesPaid = g.Select(r => r.EmployeeId).Distinct().Count(),
                AverageSalary = g.Average(r => r.Salary),
                HighestSalary = g.Max(r => r.Salary),
                LowestSalary = g.Min(r => r.Salary)
            })
            .ToList();

        // Apply sorting
        groupedByMonth = sortBy?.ToLower() switch
        {
            "totalsalarypaid" => descending ? [.. groupedByMonth.OrderByDescending(g => g.TotalSalaryPaid)] : [.. groupedByMonth.OrderBy(g => g.TotalSalaryPaid)],
            "employeespaid" => descending ? [.. groupedByMonth.OrderByDescending(g => g.EmployeesPaid)] : [.. groupedByMonth.OrderBy(g => g.EmployeesPaid)],
            _ => descending ? [.. groupedByMonth.OrderByDescending(g => g.Year).ThenByDescending(g => g.Month)] : [.. groupedByMonth.OrderBy(g => g.Year).ThenBy(g => g.Month)]
        };

        return groupedByMonth;
    }

    public async Task<List<SalaryRecord>> GetPayrollAsync(
        DateTime payDate,
        string? departmentName,
        StaffType? staffType,
        EmploymentStatus? employmentStatus)
    {
        var query = _context.SalaryRecords
            .Include(s => s.Employee)
                .ThenInclude(e => e.User)
            .Where(s => s.PayDate.Date == payDate.Date);

        if (!string.IsNullOrWhiteSpace(departmentName))
            query = query.Where(s => s.Employee.DepartmentName != null && s.Employee.DepartmentName.Contains(departmentName));

        if (staffType.HasValue)
            query = query.Where(s => s.Employee.StaffType == staffType.Value);

        if (employmentStatus.HasValue)
            query = query.Where(s => s.Employee.EmploymentStatus == employmentStatus.Value);

        return await query
            .OrderBy(s => s.Employee.DepartmentName)
            .ThenBy(s => s.Employee.User.DisplayName)
            .ToListAsync();
    }

    public async Task<SalaryRecord> AddAsync(SalaryRecord salaryRecord)
    {
        _context.SalaryRecords.Add(salaryRecord);
        await _context.SaveChangesAsync();
        return salaryRecord;
    }

    public async Task<List<SalaryRecord>> AddBatchAsync(List<SalaryRecord> salaryRecords)
    {
        _context.SalaryRecords.AddRange(salaryRecords);
        await _context.SaveChangesAsync();
        return salaryRecords;
    }

    public async Task<SalaryRecord> UpdateAsync(SalaryRecord salaryRecord)
    {
        _context.SalaryRecords.Update(salaryRecord);
        await _context.SaveChangesAsync();
        return salaryRecord;
    }

    public async Task<bool> DeleteAsync(int salaryRecordId)
    {
        var record = await _context.SalaryRecords.FindAsync(salaryRecordId);
        if (record == null)
            return false;

        _context.SalaryRecords.Remove(record);
        await _context.SaveChangesAsync();
        return true;
    }
}
