using DbApp.Domain.Entities.TicketingSystem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IPromotionRepository
{
    Task<Promotion> GetByIdAsync(int id);
    Task<Promotion> GetByIdWithDetailsAsync(int id);
    Task<List<Promotion>> GetAllAsync();
    Task<Promotion> AddAsync(Promotion promotion);
    void Update(Promotion promotion);
    void Delete(Promotion promotion);
}