using DbApp.Domain.Entities.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface ITicketTypeRepository
{
    /// <summary>
    /// Get ticket type by ID.
    /// </summary>
    Task<TicketType?> GetByIdAsync(int ticketTypeId);

    /// <summary>
    /// Get all active ticket types.
    /// </summary>
    Task<List<TicketType>> GetActiveTicketTypesAsync();

    /// <summary>
    /// Get sold ticket count for a specific ticket type on a specific date.
    /// </summary>
    Task<int> GetSoldCountAsync(int ticketTypeId, DateTime visitDate);

    /// <summary>
    /// Check if ticket type has sufficient stock for the requested quantity.
    /// </summary>
    Task<bool> HasSufficientStockAsync(int ticketTypeId, DateTime visitDate, int requestedQuantity);
}
