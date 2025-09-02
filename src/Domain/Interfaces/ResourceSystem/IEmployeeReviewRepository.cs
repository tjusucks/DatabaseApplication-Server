using DbApp.Domain.Entities.ResourceSystem;

namespace DbApp.Domain.Interfaces.ResourceSystem;

public interface IEmployeeReviewRepository
{
    // 基础CRUD操作
    Task<int> CreateAsync(EmployeeReview review);
    Task<EmployeeReview?> GetByIdAsync(int reviewId);
    Task<List<EmployeeReview>> GetAllAsync();
    Task UpdateAsync(EmployeeReview review);
    Task DeleteAsync(EmployeeReview review);

    // 查询操作
    Task<List<EmployeeReview>> GetByEmployeeAsync(int employeeId);
    Task<List<EmployeeReview>> GetByPeriodAsync(string period);
    Task<List<EmployeeReview>> GetByEvaluatorAsync(int evaluatorId);
    Task<EmployeeReview?> GetByEmployeeAndPeriodAsync(int employeeId, string period);
}
