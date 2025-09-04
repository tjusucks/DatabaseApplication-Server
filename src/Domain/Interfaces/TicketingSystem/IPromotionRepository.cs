using DbApp.Domain.Entities.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IPromotionRepository
{
    /// <summary>
    /// Get promotion by ID.
    /// </summary>
    Task<Promotion?> GetByIdAsync(int promotionId);

    /// <summary>
    /// Get all active promotions.
    /// </summary>
    Task<List<Promotion>> GetActivePromotionsAsync();

    /// <summary>
    /// Check if promotion is valid for the given ticket types and date.
    /// </summary>
    Task<bool> IsValidForTicketTypesAsync(int promotionId, List<int> ticketTypeIds, DateTime visitDate);
}
