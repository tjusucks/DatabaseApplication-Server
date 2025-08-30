using Microsoft.AspNetCore.Mvc;
using DbApp.Application.Interfaces.TicketingSystem;
using DbApp.Domain.Entities.TicketingSystem;
using System.Threading.Tasks;
using DbApp.Application.DTOs;

namespace DbApp.Presentation.Controllers;
[ApiController]
[Route("api/ticket-types")]
public class TicketTypeController : ControllerBase
{
    private readonly IPriceRepository _priceService;

    public TicketTypeController(IPriceRepository priceService)
    {
        _priceService = priceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _priceService.GetAllTicketTypesAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _priceService.GetTicketTypeByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{id}/base-price")]
    public async Task<IActionResult> UpdateBasePrice(int id, UpdateBasePriceRequest dto)
    {
        var success = await _priceService.UpdateBasePriceAsync(id, dto);
        if (!success) return BadRequest();
        return NoContent();
    }
}