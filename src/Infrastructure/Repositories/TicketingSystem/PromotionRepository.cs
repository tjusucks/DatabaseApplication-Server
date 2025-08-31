using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbApp.Infrastructure.Repositories.TicketingSystem;

public class PromotionRepository : IPromotionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PromotionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Promotion> GetByIdAsync(int id)
    {
        return await _dbContext.Promotions.FindAsync(id);
    }

    public async Task<Promotion> GetByIdWithDetailsAsync(int id)
    {
        return await _dbContext.Promotions
            .Include(p => p.PromotionTicketTypes)
                .ThenInclude(pt => pt.TicketType)
            .Include(p => p.PromotionConditions)
            .Include(p => p.PromotionActions)
            .FirstOrDefaultAsync(p => p.PromotionId == id);
    }

    public async Task<List<Promotion>> GetAllAsync()
    {
        return await _dbContext.Promotions.ToListAsync();
    }

    public async Task<Promotion> AddAsync(Promotion promotion)
    {
        await _dbContext.Promotions.AddAsync(promotion);
        return promotion;
    }

    public void Update(Promotion promotion)
    {
        _dbContext.Entry(promotion).State = EntityState.Modified;
    }

    public void Delete(Promotion promotion)
    {
        // EF Core will handle cascading deletes for related entities
        // if configured correctly in OnModelCreating.
        // Otherwise, you would remove child collections here manually.
        _dbContext.Promotions.Remove(promotion);
    }
}