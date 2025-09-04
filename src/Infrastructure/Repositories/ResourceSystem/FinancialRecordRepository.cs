using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

public class FinancialRecordRepository(ApplicationDbContext context) : IFinancialRecordRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<FinancialRecord>> SearchAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        TransactionType? transactionType,
        PaymentMethod? paymentMethod,
        int? responsibleEmployeeId,
        int? approvedById,
        decimal? minAmount,
        decimal? maxAmount,
        string? sortBy,
        bool descending,
        int page,
        int pageSize)
    {
        var query = _context.FinancialRecords
            .Include(f => f.ResponsibleEmployee)
                .ThenInclude(e => e!.User)
            .Include(f => f.ApprovedBy)
                .ThenInclude(e => e!.User)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(f =>
                (f.ResponsibleEmployee!.User.DisplayName.Contains(keyword)) ||
                (f.ResponsibleEmployee.StaffNumber.Contains(keyword)) ||
                (f.ApprovedBy!.User.DisplayName.Contains(keyword)) ||
                (f.ApprovedBy.StaffNumber.Contains(keyword)) ||
                (f.Description != null && f.Description.Contains(keyword)));
        }

        if (startDate.HasValue)
        {
            query = query.Where(f => f.TransactionDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(f => f.TransactionDate <= endDate.Value);
        }

        if (transactionType.HasValue)
        {
            query = query.Where(f => f.TransactionType == transactionType.Value);
        }

        if (paymentMethod.HasValue)
        {
            query = query.Where(f => f.PaymentMethod == paymentMethod.Value);
        }

        if (responsibleEmployeeId.HasValue)
        {
            query = query.Where(f => f.ResponsibleEmployeeId == responsibleEmployeeId.Value);
        }

        if (approvedById.HasValue)
        {
            query = query.Where(f => f.ApprovedById == approvedById.Value);
        }

        if (minAmount.HasValue)
        {
            query = query.Where(f => f.Amount >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(f => f.Amount <= maxAmount.Value);
        }

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "amount" => descending ? query.OrderByDescending(f => f.Amount) : query.OrderBy(f => f.Amount),
            "createdat" => descending ? query.OrderByDescending(f => f.CreatedAt) : query.OrderBy(f => f.CreatedAt),
            _ => descending ? query.OrderByDescending(f => f.TransactionDate) : query.OrderBy(f => f.TransactionDate)
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
        TransactionType? transactionType,
        PaymentMethod? paymentMethod,
        int? responsibleEmployeeId,
        int? approvedById,
        decimal? minAmount,
        decimal? maxAmount)
    {
        var query = _context.FinancialRecords
            .Include(f => f.ResponsibleEmployee)
                .ThenInclude(e => e!.User)
            .Include(f => f.ApprovedBy)
                .ThenInclude(e => e!.User)
            .AsQueryable();

        // Apply the same filters as SearchAsync
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(f =>
                (f.ResponsibleEmployee!.User.DisplayName.Contains(keyword)) ||
                (f.ResponsibleEmployee.StaffNumber.Contains(keyword)) ||
                (f.ApprovedBy!.User.DisplayName.Contains(keyword)) ||
                (f.ApprovedBy.StaffNumber.Contains(keyword)) ||
                (f.Description != null && f.Description.Contains(keyword)));
        }

        if (startDate.HasValue)
            query = query.Where(f => f.TransactionDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(f => f.TransactionDate <= endDate.Value);

        if (transactionType.HasValue)
            query = query.Where(f => f.TransactionType == transactionType.Value);

        if (paymentMethod.HasValue)
            query = query.Where(f => f.PaymentMethod == paymentMethod.Value);

        if (responsibleEmployeeId.HasValue)
            query = query.Where(f => f.ResponsibleEmployeeId == responsibleEmployeeId.Value);

        if (approvedById.HasValue)
            query = query.Where(f => f.ApprovedById == approvedById.Value);

        if (minAmount.HasValue)
            query = query.Where(f => f.Amount >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(f => f.Amount <= maxAmount.Value);

        return await query.CountAsync();
    }

    public async Task<FinancialRecord?> GetByIdAsync(int recordId)
    {
        return await _context.FinancialRecords
            .Include(f => f.ResponsibleEmployee)
                .ThenInclude(e => e!.User)
            .Include(f => f.ApprovedBy)
                .ThenInclude(e => e!.User)
            .FirstOrDefaultAsync(f => f.RecordId == recordId);
    }

    public async Task<FinancialStats> GetStatsAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        TransactionType? transactionType,
        PaymentMethod? paymentMethod,
        int? responsibleEmployeeId,
        int? approvedById,
        decimal? minAmount,
        decimal? maxAmount)
    {
        var query = _context.FinancialRecords
            .Include(f => f.ResponsibleEmployee)
                .ThenInclude(e => e!.User)
            .Include(f => f.ApprovedBy)
                .ThenInclude(e => e!.User)
            .AsQueryable();

        // Apply the same filters
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(f =>
                (f.ResponsibleEmployee!.User.DisplayName.Contains(keyword)) ||
                (f.ResponsibleEmployee.StaffNumber.Contains(keyword)) ||
                (f.ApprovedBy!.User.DisplayName.Contains(keyword)) ||
                (f.ApprovedBy.StaffNumber.Contains(keyword)) ||
                (f.Description != null && f.Description.Contains(keyword)));
        }

        if (startDate.HasValue)
            query = query.Where(f => f.TransactionDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(f => f.TransactionDate <= endDate.Value);

        if (transactionType.HasValue)
            query = query.Where(f => f.TransactionType == transactionType.Value);

        if (paymentMethod.HasValue)
            query = query.Where(f => f.PaymentMethod == paymentMethod.Value);

        if (responsibleEmployeeId.HasValue)
            query = query.Where(f => f.ResponsibleEmployeeId == responsibleEmployeeId.Value);

        if (approvedById.HasValue)
            query = query.Where(f => f.ApprovedById == approvedById.Value);

        if (minAmount.HasValue)
            query = query.Where(f => f.Amount >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(f => f.Amount <= maxAmount.Value);

        var records = await query.ToListAsync();

        var totalIncome = records.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount);
        var totalExpense = records.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount);
        var totalRefund = records.Where(r => r.TransactionType == TransactionType.Refund).Sum(r => r.Amount);
        var totalTransfer = records.Where(r => r.TransactionType == TransactionType.Transfer).Sum(r => r.Amount);

        return new FinancialStats
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            TotalRefund = totalRefund,
            TotalTransfer = totalTransfer,
            NetProfit = totalIncome - totalExpense,
            TotalRecords = records.Count,
            AverageTransactionAmount = records.Count > 0 ? records.Average(r => r.Amount) : 0,
            FirstTransaction = records.Count > 0 ? records.Min(r => r.TransactionDate) : null,
            LastTransaction = records.Count > 0 ? records.Max(r => r.TransactionDate) : null
        };
    }

    public async Task<List<GroupedFinancialStats>> GetGroupedStatsAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        TransactionType? transactionType,
        PaymentMethod? paymentMethod,
        int? responsibleEmployeeId,
        int? approvedById,
        decimal? minAmount,
        decimal? maxAmount,
        string groupBy,
        string? sortBy,
        bool descending)
    {
        var query = _context.FinancialRecords
            .Include(f => f.ResponsibleEmployee)
                .ThenInclude(e => e!.User)
            .Include(f => f.ApprovedBy)
                .ThenInclude(e => e!.User)
            .AsQueryable();

        // Apply filters (same as before)
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(f =>
                (f.ResponsibleEmployee!.User.DisplayName.Contains(keyword)) ||
                (f.ResponsibleEmployee.StaffNumber.Contains(keyword)) ||
                (f.ApprovedBy!.User.DisplayName.Contains(keyword)) ||
                (f.ApprovedBy.StaffNumber.Contains(keyword)) ||
                (f.Description != null && f.Description.Contains(keyword)));
        }

        if (startDate.HasValue)
            query = query.Where(f => f.TransactionDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(f => f.TransactionDate <= endDate.Value);
        if (transactionType.HasValue)
            query = query.Where(f => f.TransactionType == transactionType.Value);
        if (paymentMethod.HasValue)
            query = query.Where(f => f.PaymentMethod == paymentMethod.Value);
        if (responsibleEmployeeId.HasValue)
            query = query.Where(f => f.ResponsibleEmployeeId == responsibleEmployeeId.Value);
        if (approvedById.HasValue)
            query = query.Where(f => f.ApprovedById == approvedById.Value);
        if (minAmount.HasValue)
            query = query.Where(f => f.Amount >= minAmount.Value);
        if (maxAmount.HasValue)
            query = query.Where(f => f.Amount <= maxAmount.Value);

        var records = await query.ToListAsync();

        var groupedData = groupBy.ToLower() switch
        {
            "transactiontype" => records.GroupBy(r => new { Key = r.TransactionType.ToString(), Name = r.TransactionType.ToString() })
                .Select(g => new GroupedFinancialStats
                {
                    GroupKey = g.Key.Key,
                    GroupName = g.Key.Name,
                    TotalIncome = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount),
                    TotalExpense = g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TotalRefund = g.Where(r => r.TransactionType == TransactionType.Refund).Sum(r => r.Amount),
                    TotalTransfer = g.Where(r => r.TransactionType == TransactionType.Transfer).Sum(r => r.Amount),
                    NetProfit = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount) -
                               g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TransactionCount = g.Count(),
                    AverageAmount = g.Average(r => r.Amount),
                    FirstTransaction = g.Any() ? g.Min(r => r.TransactionDate) : null,
                    LastTransaction = g.Any() ? g.Max(r => r.TransactionDate) : null
                }),
            "paymentmethod" => records.GroupBy(r => new { Key = r.PaymentMethod?.ToString() ?? "Unknown", Name = r.PaymentMethod?.ToString() ?? "Unknown" })
                .Select(g => new GroupedFinancialStats
                {
                    GroupKey = g.Key.Key,
                    GroupName = g.Key.Name,
                    TotalIncome = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount),
                    TotalExpense = g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TotalRefund = g.Where(r => r.TransactionType == TransactionType.Refund).Sum(r => r.Amount),
                    TotalTransfer = g.Where(r => r.TransactionType == TransactionType.Transfer).Sum(r => r.Amount),
                    NetProfit = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount) -
                               g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TransactionCount = g.Count(),
                    AverageAmount = g.Average(r => r.Amount),
                    FirstTransaction = g.Any() ? g.Min(r => r.TransactionDate) : null,
                    LastTransaction = g.Any() ? g.Max(r => r.TransactionDate) : null
                }),
            "date" => records.GroupBy(r => new { Key = r.TransactionDate.ToString("yyyy-MM-dd"), Name = r.TransactionDate.ToString("yyyy-MM-dd") })
                .Select(g => new GroupedFinancialStats
                {
                    GroupKey = g.Key.Key,
                    GroupName = g.Key.Name,
                    TotalIncome = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount),
                    TotalExpense = g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TotalRefund = g.Where(r => r.TransactionType == TransactionType.Refund).Sum(r => r.Amount),
                    TotalTransfer = g.Where(r => r.TransactionType == TransactionType.Transfer).Sum(r => r.Amount),
                    NetProfit = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount) -
                               g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TransactionCount = g.Count(),
                    AverageAmount = g.Average(r => r.Amount),
                    FirstTransaction = g.Any() ? g.Min(r => r.TransactionDate) : null,
                    LastTransaction = g.Any() ? g.Max(r => r.TransactionDate) : null
                }),
            "month" => records.GroupBy(r => new { Key = r.TransactionDate.ToString("yyyy-MM"), Name = r.TransactionDate.ToString("yyyy年MM月") })
                .Select(g => new GroupedFinancialStats
                {
                    GroupKey = g.Key.Key,
                    GroupName = g.Key.Name,
                    TotalIncome = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount),
                    TotalExpense = g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TotalRefund = g.Where(r => r.TransactionType == TransactionType.Refund).Sum(r => r.Amount),
                    TotalTransfer = g.Where(r => r.TransactionType == TransactionType.Transfer).Sum(r => r.Amount),
                    NetProfit = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount) -
                               g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TransactionCount = g.Count(),
                    AverageAmount = g.Average(r => r.Amount),
                    FirstTransaction = g.Any() ? g.Min(r => r.TransactionDate) : null,
                    LastTransaction = g.Any() ? g.Max(r => r.TransactionDate) : null
                }),
            "responsibleemployee" => records.Where(r => r.ResponsibleEmployee != null)
                .GroupBy(r => new { Key = r.ResponsibleEmployeeId.ToString() ?? "Unknown", Name = r.ResponsibleEmployee!.User.DisplayName ?? "Unknown" })
                .Select(g => new GroupedFinancialStats
                {
                    GroupKey = g.Key.Key,
                    GroupName = g.Key.Name,
                    TotalIncome = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount),
                    TotalExpense = g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TotalRefund = g.Where(r => r.TransactionType == TransactionType.Refund).Sum(r => r.Amount),
                    TotalTransfer = g.Where(r => r.TransactionType == TransactionType.Transfer).Sum(r => r.Amount),
                    NetProfit = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount) -
                               g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TransactionCount = g.Count(),
                    AverageAmount = g.Average(r => r.Amount),
                    FirstTransaction = g.Any() ? g.Min(r => r.TransactionDate) : null,
                    LastTransaction = g.Any() ? g.Max(r => r.TransactionDate) : null
                }),
            _ => records.GroupBy(r => new { Key = r.TransactionType.ToString(), Name = r.TransactionType.ToString() })
                .Select(g => new GroupedFinancialStats
                {
                    GroupKey = g.Key.Key,
                    GroupName = g.Key.Name,
                    TotalIncome = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount),
                    TotalExpense = g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TotalRefund = g.Where(r => r.TransactionType == TransactionType.Refund).Sum(r => r.Amount),
                    TotalTransfer = g.Where(r => r.TransactionType == TransactionType.Transfer).Sum(r => r.Amount),
                    NetProfit = g.Where(r => r.TransactionType == TransactionType.Income).Sum(r => r.Amount) -
                               g.Where(r => r.TransactionType == TransactionType.Expense).Sum(r => r.Amount),
                    TransactionCount = g.Count(),
                    AverageAmount = g.Average(r => r.Amount),
                    FirstTransaction = g.Any() ? g.Min(r => r.TransactionDate) : null,
                    LastTransaction = g.Any() ? g.Max(r => r.TransactionDate) : null
                })
        };

        var result = groupedData.ToList();

        // Apply sorting
        result = sortBy?.ToLower() switch
        {
            "totalincome" => descending ? [.. result.OrderByDescending(g => g.TotalIncome)] : [.. result.OrderBy(g => g.TotalIncome)],
            "totalexpense" => descending ? [.. result.OrderByDescending(g => g.TotalExpense)] : [.. result.OrderBy(g => g.TotalExpense)],
            "transactioncount" => descending ? [.. result.OrderByDescending(g => g.TransactionCount)] : [.. result.OrderBy(g => g.TransactionCount)],
            "groupname" => descending ? [.. result.OrderByDescending(g => g.GroupName)] : [.. result.OrderBy(g => g.GroupName)],
            _ => descending ? [.. result.OrderByDescending(g => g.NetProfit)] : [.. result.OrderBy(g => g.NetProfit)]
        };

        return result;
    }

    public async Task<IncomeExpenseOverview> GetIncomeExpenseOverviewAsync(
        DateTime? startDate,
        DateTime? endDate,
        int? responsibleEmployeeId,
        int? approvedById)
    {
        var query = _context.FinancialRecords.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(f => f.TransactionDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(f => f.TransactionDate <= endDate.Value);

        if (responsibleEmployeeId.HasValue)
            query = query.Where(f => f.ResponsibleEmployeeId == responsibleEmployeeId.Value);

        if (approvedById.HasValue)
            query = query.Where(f => f.ApprovedById == approvedById.Value);

        var records = await query.ToListAsync();

        var incomeRecords = records.Where(r => r.TransactionType == TransactionType.Income).ToList();
        var expenseRecords = records.Where(r => r.TransactionType == TransactionType.Expense).ToList();

        var totalIncome = incomeRecords.Sum(r => r.Amount);
        var totalExpense = expenseRecords.Sum(r => r.Amount);
        var netProfit = totalIncome - totalExpense;

        return new IncomeExpenseOverview
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            NetProfit = netProfit,
            ProfitMargin = totalIncome > 0 ? (netProfit / totalIncome) * 100 : 0,
            IncomeTransactionCount = incomeRecords.Count,
            ExpenseTransactionCount = expenseRecords.Count,
            AverageIncomeAmount = incomeRecords.Count > 0 ? incomeRecords.Average(r => r.Amount) : 0,
            AverageExpenseAmount = expenseRecords.Count > 0 ? expenseRecords.Average(r => r.Amount) : 0
        };
    }

    public async Task<List<FinancialRecord>> GetByTypeAsync(
        TransactionType transactionType,
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        PaymentMethod? paymentMethod,
        int? responsibleEmployeeId,
        int? approvedById,
        decimal? minAmount,
        decimal? maxAmount,
        string? sortBy,
        bool descending,
        int page,
        int pageSize)
    {
        var query = _context.FinancialRecords
            .Include(f => f.ResponsibleEmployee)
                .ThenInclude(e => e!.User)
            .Include(f => f.ApprovedBy)
                .ThenInclude(e => e!.User)
            .Where(f => f.TransactionType == transactionType);

        // Apply filters (similar to SearchAsync but without transaction type filter)
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(f =>
                (f.ResponsibleEmployee!.User.DisplayName.Contains(keyword)) ||
                (f.ResponsibleEmployee.StaffNumber.Contains(keyword)) ||
                (f.ApprovedBy!.User.DisplayName.Contains(keyword)) ||
                (f.ApprovedBy.StaffNumber.Contains(keyword)) ||
                (f.Description != null && f.Description.Contains(keyword)));
        }

        if (startDate.HasValue)
            query = query.Where(f => f.TransactionDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(f => f.TransactionDate <= endDate.Value);
        if (paymentMethod.HasValue)
            query = query.Where(f => f.PaymentMethod == paymentMethod.Value);
        if (responsibleEmployeeId.HasValue)
            query = query.Where(f => f.ResponsibleEmployeeId == responsibleEmployeeId.Value);
        if (approvedById.HasValue)
            query = query.Where(f => f.ApprovedById == approvedById.Value);
        if (minAmount.HasValue)
            query = query.Where(f => f.Amount >= minAmount.Value);
        if (maxAmount.HasValue)
            query = query.Where(f => f.Amount <= maxAmount.Value);

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "amount" => descending ? query.OrderByDescending(f => f.Amount) : query.OrderBy(f => f.Amount),
            "createdat" => descending ? query.OrderByDescending(f => f.CreatedAt) : query.OrderBy(f => f.CreatedAt),
            _ => descending ? query.OrderByDescending(f => f.TransactionDate) : query.OrderBy(f => f.TransactionDate)
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByTypeAsync(
        TransactionType transactionType,
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        PaymentMethod? paymentMethod,
        int? responsibleEmployeeId,
        int? approvedById,
        decimal? minAmount,
        decimal? maxAmount)
    {
        var query = _context.FinancialRecords
            .Include(f => f.ResponsibleEmployee)
                .ThenInclude(e => e!.User)
            .Include(f => f.ApprovedBy)
                .ThenInclude(e => e!.User)
            .Where(f => f.TransactionType == transactionType);

        // Apply the same filters as GetByTypeAsync
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(f =>
                (f.ResponsibleEmployee!.User.DisplayName.Contains(keyword)) ||
                (f.ResponsibleEmployee.StaffNumber.Contains(keyword)) ||
                (f.ApprovedBy!.User.DisplayName.Contains(keyword)) ||
                (f.ApprovedBy.StaffNumber.Contains(keyword)) ||
                (f.Description != null && f.Description.Contains(keyword)));
        }

        if (startDate.HasValue)
            query = query.Where(f => f.TransactionDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(f => f.TransactionDate <= endDate.Value);
        if (paymentMethod.HasValue)
            query = query.Where(f => f.PaymentMethod == paymentMethod.Value);
        if (responsibleEmployeeId.HasValue)
            query = query.Where(f => f.ResponsibleEmployeeId == responsibleEmployeeId.Value);
        if (approvedById.HasValue)
            query = query.Where(f => f.ApprovedById == approvedById.Value);
        if (minAmount.HasValue)
            query = query.Where(f => f.Amount >= minAmount.Value);
        if (maxAmount.HasValue)
            query = query.Where(f => f.Amount <= maxAmount.Value);

        return await query.CountAsync();
    }

    public async Task<FinancialRecord> AddAsync(FinancialRecord financialRecord)
    {
        _context.FinancialRecords.Add(financialRecord);
        await _context.SaveChangesAsync();
        return financialRecord;
    }

    public async Task<FinancialRecord> UpdateAsync(FinancialRecord financialRecord)
    {
        _context.FinancialRecords.Update(financialRecord);
        await _context.SaveChangesAsync();
        return financialRecord;
    }

    public async Task<bool> DeleteAsync(int recordId)
    {
        var record = await _context.FinancialRecords.FindAsync(recordId);
        if (record == null)
            return false;

        _context.FinancialRecords.Remove(record);
        await _context.SaveChangesAsync();
        return true;
    }
}
