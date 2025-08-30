using Microsoft.AspNetCore.Mvc;
using DbApp.Application;
using System.Threading.Tasks;
using DbApp.Application.Interfaces.Services.TicketingSystem;
using DbApp.Application.DTOs;
namespace DbApp.Presentation.Controllers;
[ApiController]
[Route("api/ticket-types/{ticketTypeId}/price-rules")]
public class PriceRuleController : ControllerBase
{
    private readonly IPriceService _priceService;

    public PriceRuleController(IPriceService priceService)
    {
        _priceService = priceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRules(int ticketTypeId)
    {
        var result = await _priceService.GetPriceRulesAsync(ticketTypeId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int ticketTypeId, CreatePriceRuleRequest dto)
    {
        var rule = await _priceService.CreatePriceRuleAsync(ticketTypeId, dto);
        if (rule == null) return BadRequest();
        return Ok(rule);
    }
}