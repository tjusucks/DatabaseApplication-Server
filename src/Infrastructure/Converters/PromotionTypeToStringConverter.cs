using DbApp.Domain.Enums; // 确保引用了你的枚举
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
namespace DbApp.Infrastructure.Converters
{
    public class PromotionTypeToStringConverter : ValueConverter<PromotionType, string>
    {
        public PromotionTypeToStringConverter() : base(
            // 第一个委托：如何从 Model 转换为 Provider (C# enum -> string)
            v => ConvertToProvider(v),
            // 第二个委托：如何从 Provider 转换为 Model (string -> C# enum)
            v => (PromotionType)Enum.Parse(typeof(PromotionType), v.Replace("_", ""), true))
        {
        }

        // 将 switch 逻辑封装在一个静态方法中
        private static string ConvertToProvider(PromotionType value)
        {
            switch (value)
            {
            case PromotionType.DiscountPercent:
                return "DISCOUNT_PERCENT";
            case PromotionType.DiscountFixed:
                return "DISCOUNT_FIXED";
            case PromotionType.FullReduction:
                return "FULL_REDUCTION";
            case PromotionType.FullGift:
                return "FULL_GIFT";
            case PromotionType.PackageDeal:
                return "PACKAGE_DEAL";
            case PromotionType.CouponBased:
                return "COUPON_BASED";
            default:
                throw new ArgumentOutOfRangeException(nameof(value), "Invalid promotion type");
            }
        }
    }
}