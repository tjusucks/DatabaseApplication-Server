using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class PromotionRepository(ApplicationDbContext dbContext) : IPromotionRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Promotion?> GetByIdAsync(int promotionId)
    {
        return await _dbContext.Promotions
            .Include(p => p.PromotionActions)
            .Include(p => p.PromotionConditions)
            .Include(p => p.PromotionTicketTypes)
            .FirstOrDefaultAsync(p => p.PromotionId == promotionId);
    }

    public async Task<List<Promotion>> GetActivePromotionsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbContext.Promotions
            .Include(p => p.PromotionActions)
            .Include(p => p.PromotionConditions)
            .Include(p => p.PromotionTicketTypes)
            .Where(p => p.IsActive && 
                       p.StartDatetime <= now && 
                       p.EndDatetime >= now)
            .OrderBy(p => p.DisplayPriority)
            .ToListAsync();
    }

    public async Task<bool> IsValidForTicketTypesAsync(int promotionId, List<int> ticketTypeIds, DateTime visitDate)
    {
        var promotion = await GetByIdAsync(promotionId);
        if (promotion == null || !promotion.IsActive) return false;

        var now = DateTime.UtcNow;
        if (promotion.StartDatetime > now || promotion.EndDatetime < now) return false;

        // 如果促销适用于所有票型
        if (promotion.AppliesToAllTickets) return true;

        // 检查是否适用于指定的票型
        var applicableTicketTypes = promotion.PromotionTicketTypes.Select(ptt => ptt.TicketTypeId).ToList();
        return ticketTypeIds.Any(ttId => applicableTicketTypes.Contains(ttId));
    }
}
