using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;

namespace DbApp.Infrastructure.Services.ResourceSystem;

/// <summary>
/// Service implementation for coordinating salary and financial record operations
/// </summary>
public class SalaryFinancialService(
    ISalaryRecordRepository salaryRecordRepository,
    IFinancialRecordRepository financialRecordRepository) : ISalaryFinancialService
{
    private readonly ISalaryRecordRepository _salaryRecordRepository = salaryRecordRepository;
    private readonly IFinancialRecordRepository _financialRecordRepository = financialRecordRepository;

    /// <summary>
    /// Creates a salary record and its corresponding financial record
    /// </summary>
    /// <param name="salaryRecord">The salary record to create</param>
    /// <returns>The created salary record with its ID</returns>
    public async Task<SalaryRecord> CreateSalaryWithFinancialRecordAsync(SalaryRecord salaryRecord)
    {
        // Create the salary record first
        var createdSalaryRecord = await _salaryRecordRepository.AddAsync(salaryRecord);

        // Create the corresponding financial record
        var financialRecord = new FinancialRecord
        {
            TransactionDate = salaryRecord.PayDate,
            Amount = salaryRecord.Salary,
            TransactionType = TransactionType.Expense, // Salaries are expenses
            PaymentMethod = PaymentMethod.Cash, // Default to cash for salary payments
            ResponsibleEmployeeId = salaryRecord.EmployeeId,
            Description = $"Salary payment for employee {salaryRecord.EmployeeId} on {salaryRecord.PayDate:yyyy-MM-dd}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _financialRecordRepository.AddAsync(financialRecord);

        return createdSalaryRecord;
    }

    /// <summary>
    /// Creates a batch of salary records with their corresponding financial records
    /// </summary>
    /// <param name="salaryRecords">The salary records to create</param>
    /// <returns>The created salary records with their IDs</returns>
    public async Task<List<SalaryRecord>> CreateBatchSalariesWithFinancialRecordsAsync(List<SalaryRecord> salaryRecords)
    {
        // Create all salary records first
        var createdSalaryRecords = await _salaryRecordRepository.AddBatchAsync(salaryRecords);

        // Create corresponding financial records for each salary record
        var financialRecords = createdSalaryRecords.Select(salaryRecord => new FinancialRecord
        {
            TransactionDate = salaryRecord.PayDate,
            Amount = salaryRecord.Salary,
            TransactionType = TransactionType.Expense, // Salaries are expenses
            PaymentMethod = PaymentMethod.Cash, // Default to cash for salary payments
            ResponsibleEmployeeId = salaryRecord.EmployeeId,
            Description = $"Salary payment for employee {salaryRecord.EmployeeId} on {salaryRecord.PayDate:yyyy-MM-dd}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        foreach (var financialRecord in financialRecords)
        {
            await _financialRecordRepository.AddAsync(financialRecord);
        }

        return createdSalaryRecords;
    }
}
