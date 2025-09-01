using DbApp.Domain.Entities.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IPromotionConditionRepository
{
    Task<PromotionCondition?> GetByIdAsync(int id);
    Task<List<PromotionCondition>> GetByPromotionIdAsync(int promotionId);
    Task<int> CreateAsync(PromotionCondition condition);
    Task UpdateAsync(PromotionCondition condition);
    Task DeleteAsync(PromotionCondition condition);
}
