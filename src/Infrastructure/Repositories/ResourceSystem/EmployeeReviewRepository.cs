using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.ResourceSystem;

public class EmployeeReviewRepository(ApplicationDbContext dbContext) : IEmployeeReviewRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(EmployeeReview review)
    {
        _dbContext.EmployeeReviews.Add(review);
        await _dbContext.SaveChangesAsync();
        return review.ReviewId;
    }

    public async Task<EmployeeReview?> GetByIdAsync(int reviewId)
    {
        return await _dbContext.EmployeeReviews
            .Include(r => r.Employee)
            .Include(r => r.Evaluator)
            .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
    }

    public async Task<List<EmployeeReview>> GetAllAsync()
    {
        return await _dbContext.EmployeeReviews
            .Include(r => r.Employee)
            .Include(r => r.Evaluator)
            .ToListAsync();
    }

    public async Task UpdateAsync(EmployeeReview review)
    {
        _dbContext.EmployeeReviews.Update(review);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(EmployeeReview review)
    {
        _dbContext.EmployeeReviews.Remove(review);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<EmployeeReview>> GetByEmployeeAsync(int employeeId)
    {
        return await _dbContext.EmployeeReviews
            .Include(r => r.Employee)
            .Include(r => r.Evaluator)
            .Where(r => r.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<List<EmployeeReview>> GetByPeriodAsync(string period)
    {
        return await _dbContext.EmployeeReviews
            .Include(r => r.Employee)
            .Include(r => r.Evaluator)
            .Where(r => r.Period == period)
            .ToListAsync();
    }

    public async Task<List<EmployeeReview>> GetByEvaluatorAsync(int evaluatorId)
    {
        return await _dbContext.EmployeeReviews
            .Include(r => r.Employee)
            .Include(r => r.Evaluator)
            .Where(r => r.EvaluatorId == evaluatorId)
            .ToListAsync();
    }

    public async Task<EmployeeReview?> GetByEmployeeAndPeriodAsync(int employeeId, string period)
    {
        return await _dbContext.EmployeeReviews
            .Include(r => r.Employee)
            .Include(r => r.Evaluator)
            .Where(r => r.EmployeeId == employeeId && r.Period == period)
            .FirstOrDefaultAsync();
    }
}
