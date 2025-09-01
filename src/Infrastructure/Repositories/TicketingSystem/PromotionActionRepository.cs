using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class PromotionActionRepository(ApplicationDbContext dbContext) : IPromotionActionRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<PromotionAction?> GetByIdAsync(int actionId)
    {
        return await _dbContext.PromotionActions.FindAsync(actionId);
    }
    public async Task<List<PromotionAction>> GetByPromotionIdAsync(int promotionId)
    {
        return await _dbContext.PromotionActions
            .Where(a => a.PromotionId == promotionId)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(PromotionAction action)
    {
        await _dbContext.PromotionActions.AddAsync(action);
        await _dbContext.SaveChangesAsync();
        return action.ActionId;
    }

    public async Task UpdateAsync(PromotionAction action)
    {
        _dbContext.Entry(action).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(PromotionAction action)
    {
        _dbContext.PromotionActions.Remove(action);
        await _dbContext.SaveChangesAsync();
    }
}
