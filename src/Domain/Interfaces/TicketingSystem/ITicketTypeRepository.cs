using DbApp.Domain.Entities.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface ITicketTypeRepository
{
    Task<TicketType?> GetByIdAsync(int ticketTypeId);
    Task<List<TicketType>> GetAllAsync();
    Task<int> CreateAsync(TicketType ticketType);
    Task UpdateAsync(TicketType ticketType);
    Task DeleteAsync(TicketType ticketType);
}
