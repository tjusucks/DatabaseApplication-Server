using DbApp.Domain.Entities.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

/// <summary>
/// Service interface for coordinating salary and financial record operations
/// </summary>
public interface ISalaryFinancialService
{
    /// <summary>
    /// Creates a salary record and its corresponding financial record
    /// </summary>
    /// <param name="salaryRecord">The salary record to create</param>
    /// <returns>The created salary record with its ID</returns>
    Task<SalaryRecord> CreateSalaryWithFinancialRecordAsync(SalaryRecord salaryRecord);
    
    /// <summary>
    /// Creates a batch of salary records with their corresponding financial records
    /// </summary>
    /// <param name="salaryRecords">The salary records to create</param>
    /// <returns>The created salary records with their IDs</returns>
    Task<List<SalaryRecord>> CreateBatchSalariesWithFinancialRecordsAsync(List<SalaryRecord> salaryRecords);
}