using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using Microsoft.EntityFrameworkCore;
// ... other usings

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class PromotionActionRepository : IPromotionActionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PromotionActionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PromotionAction> GetByIdAsync(int id)
    {
        return await _dbContext.PromotionActions.FindAsync(id);
    }
    public async Task<List<PromotionAction>> GetByPromotionIdAsync(int promotionId)
    {
        return await _dbContext.PromotionActions
            .Where(a => a.PromotionId == promotionId)
            .ToListAsync();
    }

    public async Task<PromotionAction> AddAsync(PromotionAction action)
    {
        await _dbContext.PromotionActions.AddAsync(action);
        // New: Save changes here
        await _dbContext.SaveChangesAsync();
        return action;
    }

    public async Task UpdateAsync(PromotionAction action)
    {
        _dbContext.Entry(action).State = EntityState.Modified;
        // New: Save changes here
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(PromotionAction action)
    {
        _dbContext.PromotionActions.Remove(action);
        // New: Save changes here
        await _dbContext.SaveChangesAsync();
    }
}