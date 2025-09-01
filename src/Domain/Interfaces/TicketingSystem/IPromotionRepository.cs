using DbApp.Domain.Entities.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IPromotionRepository
{
    Task<Promotion?> GetByIdAsync(int promotionId);
    Task<Promotion?> GetByIdWithDetailsAsync(int promotionId);
    Task<List<Promotion>> GetAllAsync();
    Task<int> CreateAsync(Promotion promotion);
    Task UpdateAsync(Promotion promotion);
    Task DeleteAsync(Promotion promotion);
}
