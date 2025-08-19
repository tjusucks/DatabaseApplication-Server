
namespace DbApp.Domain.Enums
{
    public enum PromotionActionType
    {
        PercentOff,     // 'PERCENT_OFF' - 百分比折扣
        AmountOff,      // 'AMOUNT_OFF' - 固定金额减免
        FixedPrice,     // 'FIXED_PRICE' - 固定价格
        FreeTicket,     // 'FREE_TICKET' - 赠送票
        GiftPoints      // 'GIFT_POINTS' - 赠送积分
    }
}