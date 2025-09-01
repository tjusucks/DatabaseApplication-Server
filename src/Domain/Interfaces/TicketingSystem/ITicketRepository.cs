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
}
