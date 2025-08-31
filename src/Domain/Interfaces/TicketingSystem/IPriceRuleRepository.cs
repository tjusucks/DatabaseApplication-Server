using DbApp.Domain.Entities.TicketingSystem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IPriceRuleRepository
{
    Task<PriceRule> GetByIdAsync(int id);
    Task<List<PriceRule>> GetByTicketTypeIdAsync(int ticketTypeId);
    Task<PriceRule> AddAsync(PriceRule priceRule);
    void Update(PriceRule priceRule); // EF Core tracks changes, so often void is fine
    void Delete(PriceRule priceRule); // The actual save is handled by UnitOfWork
}