using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class PriceRuleRepository(ApplicationDbContext dbContext) : IPriceRuleRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<PriceRule?> GetByIdAsync(int priceRuleId)
    {
        return await _dbContext.PriceRules.FindAsync(priceRuleId);
    }

    public async Task<List<PriceRule>> GetByTicketTypeIdAsync(int ticketTypeId)
    {
        return await _dbContext.PriceRules
            .Where(r => r.TicketTypeId == ticketTypeId)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(PriceRule priceRule)
    {
        await _dbContext.PriceRules.AddAsync(priceRule);
        await _dbContext.SaveChangesAsync();
        return priceRule.PriceRuleId;
    }

    public async Task UpdateAsync(PriceRule priceRule)
    {
        _dbContext.PriceRules.Update(priceRule);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(PriceRule priceRule)
    {
        _dbContext.PriceRules.Remove(priceRule);
        await _dbContext.SaveChangesAsync();
    }
}
