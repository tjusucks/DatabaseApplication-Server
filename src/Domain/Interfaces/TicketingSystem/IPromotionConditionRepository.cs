using DbApp.Domain.Entities.TicketingSystem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IPromotionConditionRepository
{
    Task<PromotionCondition> GetByIdAsync(int id);
    Task<List<PromotionCondition>> GetByPromotionIdAsync(int promotionId);
    Task<PromotionCondition> AddAsync(PromotionCondition condition);
    // Refactored: Changed return type from void to Task
    Task UpdateAsync(PromotionCondition condition);
    // Refactored: Changed parameter from int to entity and return type to Task
    Task DeleteAsync(PromotionCondition condition);
}