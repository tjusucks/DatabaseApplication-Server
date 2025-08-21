using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Entities.UserSystem;

namespace DbApp.Domain.Entities.TicketingSystem;

public class PriceRule
{
    public int PriceRuleId { get; set; }
    public int TicketTypeId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public int Priority { get; set; }
    public DateTime EffectiveStartDate { get; set; }
    public DateTime EffectiveEndDate { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }

    [Range(0.0, double.MaxValue)]
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public int? CreatedById { get; set; }

    // 导航属性
    public TicketType TicketType { get; set; } = null!;
    public Employee? CreatedBy { get; set; }
}
