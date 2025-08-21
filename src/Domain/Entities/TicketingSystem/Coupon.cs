using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Domain.Entities.TicketingSystem;

public class Coupon
{
    public int CouponId { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public int PromotionId { get; set; }
    public CouponDiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinPurchaseAmount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsUsed { get; set; }
    public int? UsedById { get; set; }
    public DateTime? UsedTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public Promotion Promotion { get; set; } = null!;
    public Visitor? UsedBy { get; set; }
}

