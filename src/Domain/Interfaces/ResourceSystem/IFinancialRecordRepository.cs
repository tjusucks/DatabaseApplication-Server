using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Statistics.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IFinancialRecordRepository
{
    Task<List<FinancialRecord>> SearchAsync(
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
        int pageSize);

    Task<int> CountAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        TransactionType? transactionType,
        PaymentMethod? paymentMethod,
        int? responsibleEmployeeId,
        int? approvedById,
        decimal? minAmount,
        decimal? maxAmount);

    Task<FinancialRecord?> GetByIdAsync(int recordId);

    Task<FinancialStats> GetStatsAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        TransactionType? transactionType,
        PaymentMethod? paymentMethod,
        int? responsibleEmployeeId,
        int? approvedById,
        decimal? minAmount,
        decimal? maxAmount);

    Task<List<GroupedFinancialStats>> GetGroupedStatsAsync(
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
        bool descending);

    Task<IncomeExpenseOverview> GetIncomeExpenseOverviewAsync(
        DateTime? startDate,
        DateTime? endDate,
        int? responsibleEmployeeId,
        int? approvedById);

    Task<List<FinancialRecord>> GetByTypeAsync(
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
        int pageSize);

    Task<int> CountByTypeAsync(
        TransactionType transactionType,
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        PaymentMethod? paymentMethod,
        int? responsibleEmployeeId,
        int? approvedById,
        decimal? minAmount,
        decimal? maxAmount);

    Task<FinancialRecord> AddAsync(FinancialRecord financialRecord);
    Task<FinancialRecord> UpdateAsync(FinancialRecord financialRecord);
    Task<bool> DeleteAsync(int recordId);
}
