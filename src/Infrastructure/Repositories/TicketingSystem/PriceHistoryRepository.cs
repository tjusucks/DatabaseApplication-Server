using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

/// <summary>
/// Implements the repository for handling price history data using Entity Framework Core.
/// </summary>
public class PriceHistoryRepository(ApplicationDbContext dbContext) : IPriceHistoryRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    /// <summary>
    /// Adds a new price history record to the DbContext.
    /// </summary>
    public async Task<PriceHistory> AddAsync(PriceHistory priceHistory)
    {
        await _dbContext.PriceHistories.AddAsync(priceHistory);
        return priceHistory;
    }
}
