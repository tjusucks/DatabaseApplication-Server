using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class PriceRuleRepository : IPriceRuleRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PriceRuleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PriceRule> GetByIdAsync(int id)
    {
        return await _dbContext.PriceRules.FindAsync(id);
    }

    public async Task<List<PriceRule>> GetByTicketTypeIdAsync(int ticketTypeId)
    {
        return await _dbContext.PriceRules
            .Where(r => r.TicketTypeId == ticketTypeId)
            .ToListAsync();
    }

    public async Task<PriceRule> AddAsync(PriceRule priceRule)
    {
        await _dbContext.PriceRules.AddAsync(priceRule);
        return priceRule;
    }

    public void Update(PriceRule priceRule)
    {
        _dbContext.Entry(priceRule).State = EntityState.Modified;
    }

    public void Delete(PriceRule priceRule)
    {
        _dbContext.PriceRules.Remove(priceRule);
    }
}