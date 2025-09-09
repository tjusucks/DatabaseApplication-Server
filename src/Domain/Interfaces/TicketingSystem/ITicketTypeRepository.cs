using DbApp.Domain.Entities.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface ITicketTypeRepository
{
    Task<TicketType?> GetByIdAsync(int ticketTypeId);
    Task<List<TicketType>> GetAllAsync();
    Task<List<TicketType>> GetByIdsAsync(IEnumerable<int> ticketTypeIds);
    Task<int> CreateAsync(TicketType ticketType);
    Task UpdateAsync(TicketType ticketType);
    Task DeleteAsync(TicketType ticketType);
    Task<List<TicketType>> GetActiveTicketTypesAsync();
    Task<int> GetSoldCountAsync(int ticketTypeId, DateTime visitDate);
    Task<bool> HasSufficientStockAsync(int ticketTypeId, DateTime visitDate, int requestedQuantity);
}
