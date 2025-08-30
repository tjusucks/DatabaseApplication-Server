using System.Collections.Generic;
using System.Threading.Tasks;
using DbApp.Application.DTOs;

namespace DbApp.Application.Interfaces.Services.TicketingSystem;

public interface IPromotionService
{
    Task<List<PromotionSummaryDto>> GetAllPromotionsAsync();
    Task<PromotionDetailDto> GetPromotionDetailAsync(int id);
    Task<PromotionDetailDto> CreatePromotionAsync(CreatePromotionRequest dto);
    Task<PromotionDetailDto> UpdatePromotionAsync(int id, UpdatePromotionRequest dto);
    Task<bool> DeletePromotionAsync(int id);
}
