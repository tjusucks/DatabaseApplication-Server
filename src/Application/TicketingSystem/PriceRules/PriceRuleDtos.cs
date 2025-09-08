namespace DbApp.Application.TicketingSystem.PriceRules;

public class PriceRuleDto
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
}
