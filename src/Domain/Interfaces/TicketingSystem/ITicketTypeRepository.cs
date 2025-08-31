using DbApp.Domain.Entities.TicketingSystem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface ITicketTypeRepository
{
    Task<TicketType> GetByIdAsync(int id);
    Task<List<TicketType>> GetAllAsync();
    Task<TicketType> AddAsync(TicketType ticketType);
    Task UpdateAsync(TicketType ticketType); // Often returns void or Task
    Task DeleteAsync(int id);
}
