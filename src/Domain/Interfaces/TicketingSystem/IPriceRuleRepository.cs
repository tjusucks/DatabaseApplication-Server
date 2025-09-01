using DbApp.Domain.Entities.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IPriceRuleRepository
{
    Task<PriceRule?> GetByIdAsync(int priceRuleId);
    Task<List<PriceRule>> GetByTicketTypeIdAsync(int ticketTypeId);
    Task<int> CreateAsync(PriceRule priceRule);
    Task UpdateAsync(PriceRule priceRule);
    Task DeleteAsync(PriceRule priceRule);
}
