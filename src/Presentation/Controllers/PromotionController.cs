using Microsoft.AspNetCore.Mvc;
using DbApp.Application;
using System.Threading.Tasks;
using DbApp.Application.Interfaces.TicketingSystem;
using DbApp.Application.DTOs;
namespace DbApp.Presentation.Controllers;
[ApiController]
[Route("api/promotions")]
public class PromotionController : ControllerBase
{
    private readonly IPromotionRepository _promotionService;

    public PromotionController(IPromotionRepository promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _promotionService.GetAllPromotionsAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePromotionRequest dto)
    {
        var promotion = await _promotionService.CreatePromotionAsync(dto);
        if (promotion == null) return BadRequest();
        return Ok(promotion);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var detail = await _promotionService.GetPromotionDetailAsync(id);
        if (detail == null) return NotFound();
        return Ok(detail);
    }
}