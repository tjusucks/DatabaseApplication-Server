using DbApp.Domain.Enums;


namespace DbApp.Domain.Entities.TicketRelated
{
    public class Coupon
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public int PromotionId { get; set; }
        public CouponDiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinPurchaseAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsUsed { get; set; }
        public int? UsedById { get; set; } // 使用者ID，可空
        public DateTime? UsedTime { get; set; } // 使用时间，可空
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 导航属性
        public Promotion Promotion { get; set; }
        public Visitor? UsedBy { get; set; }
    }
}