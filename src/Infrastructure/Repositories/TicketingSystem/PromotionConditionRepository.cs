using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class PromotionConditionRepository(ApplicationDbContext dbContext) : IPromotionConditionRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<PromotionCondition?> GetByIdAsync(int id)
    {
        return await _dbContext.PromotionConditions.FindAsync(id);
    }

    public async Task<List<PromotionCondition>> GetByPromotionIdAsync(int promotionId)
    {
        return await _dbContext.PromotionConditions
            .Where(c => c.PromotionId == promotionId)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(PromotionCondition condition)
    {
        await _dbContext.PromotionConditions.AddAsync(condition);
        // New: Save changes here
        await _dbContext.SaveChangesAsync();
        return condition.ConditionId;
    }

    public async Task UpdateAsync(PromotionCondition condition)
    {
        _dbContext.Entry(condition).State = EntityState.Modified;
        // New: Save changes here
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(PromotionCondition condition)
    {
        _dbContext.PromotionConditions.Remove(condition);
        // New: Save changes here
        await _dbContext.SaveChangesAsync();
    }
}
