namespace DbApp.Domain.Entities.TicketingSystem;

using DbApp.Domain.Entities.UserSystem;

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
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedById { get; set; }

    // 导航属性
    public TicketType TicketType { get; set; } = null!;

    public Employee? CreatedBy { get; set; }
}

