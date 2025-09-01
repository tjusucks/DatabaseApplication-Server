using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class TicketTypeRepository(ApplicationDbContext dbContext) : ITicketTypeRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<TicketType?> GetByIdAsync(int ticketTypeId)
    {
        return await _dbContext.TicketTypes.FindAsync(ticketTypeId);
    }

    public async Task<List<TicketType>> GetAllAsync()
    {
        return await _dbContext.TicketTypes.ToListAsync();
    }

    public async Task<int> CreateAsync(TicketType ticketType)
    {
        await _dbContext.TicketTypes.AddAsync(ticketType);
        await _dbContext.SaveChangesAsync();
        return ticketType.TicketTypeId;
    }

    public async Task UpdateAsync(TicketType ticketType)
    {
        _dbContext.TicketTypes.Update(ticketType);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(TicketType ticketType)
    {
        _dbContext.TicketTypes.Remove(ticketType);
        await _dbContext.SaveChangesAsync();
    }
}
