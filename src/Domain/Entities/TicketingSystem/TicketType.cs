using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Domain.Entities.TicketingSystem;

public class TicketType
{
    public int TicketTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal BasePrice { get; set; }
    public string? RulesText { get; set; }
    public int? MaxSaleLimit { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public ApplicableCrowd ApplicableCrowd { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = [];
    public ICollection<PromotionTicketType> ApplicablePromotions { get; set; } = [];
    public ICollection<PriceRule> PriceRules { get; set; } = [];
}
