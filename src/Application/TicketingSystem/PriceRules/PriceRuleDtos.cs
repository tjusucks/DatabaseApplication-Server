namespace DbApp.Application.TicketingSystem.PriceRules;

public class PriceRuleDto
{
    public int Id { get; set; }
    public string RuleName { get; set; }
    public int Priority { get; set; }
    public decimal Price { get; set; }
    public DateTime EffectiveStartDate { get; set; }
    public DateTime EffectiveEndDate { get; set; }
    // 其他字段...
}

    public class CreatePriceRuleRequest
    {
        public string RuleName { get; set; }
        public int Priority { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime EffectiveEndDate { get; set; }
        // 其他字段...
    }
    public class UpdatePriceRuleRequest
    {
        public string RuleName { get; set; } = string.Empty;
        public int Priority { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime EffectiveEndDate { get; set; }
    }
