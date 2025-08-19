using DbApp.Domain.Enums;

namespace DbApp.Domain.Entities.TicketRelated
{
    public class PromotionCondition
    {
        public int ConditionId { get; set; }
        public int PromotionId { get; set; }
        public string ConditionName { get; set; }
        public ConditionType ConditionType { get; set; }
        public int? TicketTypeId { get; set; } // 适用于特定票种时
        public int? MinQuantity { get; set; }
        public decimal? MinAmount { get; set; }
        public string? VisitorType { get; set; }
        public string? MemberLevel { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? DayOfWeek { get; set; } // 1-7, 对应周一到周日
        public int Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 导航属性
        public Promotion Promotion { get; set; }
        public TicketType? TicketType { get; set; }
    }
}