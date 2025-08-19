
namespace DbApp.Domain.Enums
{
    public enum PromotionType
    {
        DiscountPercent,  // 'DISCOUNT_PERCENT' - 百分比折扣
        DiscountFixed,    // 'DISCOUNT_FIXED' - 固定金额折扣
        FullReduction,    // 'FULL_REDUCTION' - 满减
        FullGift,         // 'FULL_GIFT' - 满赠
        PackageDeal,      // 'PACKAGE_DEAL' - 套餐优惠
        CouponBased       // 'COUPON_BASED' - 基于优惠券
    }
}