using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Application.TicketingSystem.TicketTypes;

public class TicketTypeDto
{
    public int TicketTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string? RulesText { get; set; }
    public int? MaxSaleLimit { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ApplicableCrowd ApplicableCrowd { get; set; }
}
