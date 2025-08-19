using DbApp.Domain.Enums;
namespace DbApp.Domain.Entities.TicketRelated
{
    public class PromotionAction
    {
        public int ActionId { get; set; }
        public int PromotionId { get; set; }
        public string ActionName { get; set; }
        public PromotionActionType ActionType { get; set; }
        public int? TargetTicketTypeId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? FixedPrice { get; set; }
        public int? PointsAwarded { get; set; }
        public int? FreeTicketTypeId { get; set; }
        public int? FreeTicketQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 导航属性
        public Promotion Promotion { get; set; }

        // 目标票种：这个动作作用于哪个票种
        public TicketType? TargetTicketType { get; set; }

        // 赠送票种：如果动作是赠送票，赠送的是哪个种类的票
        public TicketType? FreeTicketType { get; set; }
    }
}