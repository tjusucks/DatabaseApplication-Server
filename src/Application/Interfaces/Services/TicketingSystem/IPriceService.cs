using System.Collections.Generic;
using System.Threading.Tasks;
using DbApp.Application.DTOs;


namespace DbApp.Application.Interfaces.Services.TicketingSystem
{
    public interface IPriceService
    {
        Task<List<TicketTypeSummaryDto>> GetAllTicketTypesAsync();
        Task<TicketTypeSummaryDto> GetTicketTypeByIdAsync(int id);
        Task<bool> UpdateBasePriceAsync(int ticketTypeId, UpdateBasePriceRequest dto);
        Task<List<PriceRuleDto>> GetPriceRulesAsync(int ticketTypeId);
        Task<PriceRuleDto> CreatePriceRuleAsync(int ticketTypeId, CreatePriceRuleRequest dto);
        Task<PriceRuleDto> UpdatePriceRuleAsync(int ruleId, UpdatePriceRuleRequest dto);
        Task<bool> DeletePriceRuleAsync(int ruleId);
    }
}