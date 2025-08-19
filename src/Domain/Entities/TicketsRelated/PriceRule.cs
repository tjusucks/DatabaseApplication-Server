
namespace DbApp.Domain.Entities.TicketRelated
{
    public class PriceRule
    {
        public int PriceRuleId { get; set; }
        public int TicketTypeId { get; set; }
        public string RuleName { get; set; }
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
        public TicketType TicketType { get; set; }

        // 假设你有一个 Employee 实体
        public Employee? CreatedBy { get; set; }
    }
}