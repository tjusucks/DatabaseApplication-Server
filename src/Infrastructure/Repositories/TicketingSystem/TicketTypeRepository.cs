using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class TicketTypeRepository : ITicketTypeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TicketTypeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets a single ticket type by its primary key.
    /// </summary>
    public async Task<TicketType> GetByIdAsync(int id)
    {
        // FindAsync is optimized for finding by primary key.
        return await _dbContext.TicketTypes.FindAsync(id);
    }

    /// <summary>
    /// Gets a list of all ticket types.
    /// </summary>
    public async Task<List<TicketType>> GetAllAsync()
    {
        return await _dbContext.TicketTypes.ToListAsync();
    }
    
    /// <summary>
    /// Adds a new ticket type to the database context.
    /// The change is not saved until IUnitOfWork.SaveChangesAsync() is called.
    /// </summary>
    public async Task<TicketType> AddAsync(TicketType ticketType)
    {
        await _dbContext.TicketTypes.AddAsync(ticketType);
        return ticketType; // Return the entity that was added.
    }

    /// <summary>
    /// Marks an existing ticket type as modified in the database context.
    /// The change is not saved until IUnitOfWork.SaveChangesAsync() is called.
    /// </summary>
    public Task UpdateAsync(TicketType ticketType)
    {
        // Setting the state is a synchronous operation. It does not involve I/O.
        // We do not use 'async/await' here.
        _dbContext.Entry(ticketType).State = EntityState.Modified;
        
        // We return a completed task to satisfy the interface's Task return type.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Finds a ticket type by ID and marks it for deletion in the database context.
    /// The change is not saved until IUnitOfWork.SaveChangesAsync() is called.
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var ticketTypeToDelete = await _dbContext.TicketTypes.FindAsync(id);
        if (ticketTypeToDelete != null)
        {
            // Remove is a synchronous operation that marks the entity for deletion.
            _dbContext.TicketTypes.Remove(ticketTypeToDelete);
        }
        // The method signature is `async Task` so it implicitly returns a Task.
        // No explicit return is needed.
    }
}