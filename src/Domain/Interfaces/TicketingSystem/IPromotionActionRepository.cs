using DbApp.Domain.Entities.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IPromotionActionRepository
{
    Task<PromotionAction?> GetByIdAsync(int actionId);
    Task<List<PromotionAction>> GetByPromotionIdAsync(int promotionId);
    Task<int> CreateAsync(PromotionAction action);
    Task UpdateAsync(PromotionAction action);
    Task DeleteAsync(PromotionAction action);
}
