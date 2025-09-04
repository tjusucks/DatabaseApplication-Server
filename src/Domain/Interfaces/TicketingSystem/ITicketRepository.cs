using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Specifications.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface ITicketRepository
{
    /// <summary>
    /// Search ticket sale records with filtering and pagination.
    /// </summary>
    Task<List<Ticket>> SearchAsync(TicketSaleSearchSpec spec);

    /// <summary>
    /// Get total count of ticket sale records with filtering.
    /// </summary>
    Task<int> CountAsync(TicketSaleCountSpec spec);

    /// <summary>
    /// Get overall ticket sale statistics with filtering.
    /// </summary>
    Task<TicketSaleStats> GetStatsAsync(TicketSaleStatsSpec spec);

    /// <summary>
    /// Get grouped ticket sale statistics by specified dimension with filtering.
    /// </summary>
    Task<List<GroupedTicketSaleStats>> GetGroupedStatsAsync(TicketSaleGroupedStatsSpec spec);

    /// <summary>
    /// Add a new ticket to the repository.
    /// </summary>
    Task<Ticket> AddAsync(Ticket ticket);

    /// <summary>
    /// Update an existing ticket.
    /// </summary>
    Task<Ticket> UpdateAsync(Ticket ticket);

    /// <summary>
    /// Get ticket by ID.
    /// </summary>
    Task<Ticket?> GetByIdAsync(int ticketId);

    /// <summary>
    /// Get tickets by serial number.
    /// </summary>
    Task<Ticket?> GetBySerialNumberAsync(string serialNumber);

    /// <summary>
    /// Get tickets by reservation item ID.
    /// </summary>
    Task<List<Ticket>> GetByReservationItemIdAsync(int reservationItemId);

    /// <summary>
    /// Delete a ticket by ID.
    /// </summary>
    Task<bool> DeleteAsync(int ticketId);

    /// <summary>
    /// Check if ticket can be refunded (business rules).
    /// </summary>
    Task<bool> CanRefundAsync(int ticketId);
}
