using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using System.Threading.Tasks;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

/// <summary>
/// Implements the repository for handling price history data using Entity Framework Core.
/// </summary>
public class PriceHistoryRepository : IPriceHistoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PriceHistoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds a new price history record to the DbContext.
    /// </summary>
    public async Task<PriceHistory> AddAsync(PriceHistory priceHistory)
    {
        await _dbContext.PriceHistories.AddAsync(priceHistory);
        return priceHistory;
    }
}