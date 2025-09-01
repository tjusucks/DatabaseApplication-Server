using DbApp.Domain.Entities.TicketingSystem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IPromotionActionRepository
{
    Task<PromotionAction> GetByIdAsync(int id);
    Task<List<PromotionAction>> GetByPromotionIdAsync(int promotionId);
    Task<PromotionAction> AddAsync(PromotionAction action);
    // Refactored: Changed return type from void to Task
    Task UpdateAsync(PromotionAction action);
    // Refactored: Changed parameter from int to entity and return type to Task
    Task DeleteAsync(PromotionAction action);
}