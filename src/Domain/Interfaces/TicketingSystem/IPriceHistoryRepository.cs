using DbApp.Domain.Entities.TicketingSystem;
using System.Threading.Tasks;

namespace DbApp.Domain.Interfaces.TicketingSystem;

/// <summary>
/// Interface for the repository handling price history data.
/// </summary>
public interface IPriceHistoryRepository
{
    /// <summary>
    /// Adds a new price history record.
    /// </summary>
    /// <param name="priceHistory">The price history entity to add.</param>
    /// <returns>The added price history entity.</returns>
    Task<PriceHistory> AddAsync(PriceHistory priceHistory);
}