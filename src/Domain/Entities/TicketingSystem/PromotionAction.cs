using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Domain.Entities.TicketingSystem;

public class PromotionAction
{
    public int ActionId { get; set; }
    public int PromotionId { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public PromotionActionType ActionType { get; set; }
    public int? TargetTicketTypeId { get; set; }

    [Range(0.0, 100.0)]
    public decimal? DiscountPercentage { get; set; }

    [Range(0.0, double.MaxValue)]
    public decimal? DiscountAmount { get; set; }

    [Range(0.0, double.MaxValue)]
    public decimal? FixedPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int? PointsAwarded { get; set; }

    public int? FreeTicketTypeId { get; set; }

    [Range(1, int.MaxValue)]
    public int? FreeTicketQuantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public Promotion Promotion { get; set; } = null!;

    // 目标票种：这个动作作用于哪个票种
    public TicketType? TargetTicketType { get; set; }

    // 赠送票种：如果动作是赠送票，赠送的是哪个种类的票
    public TicketType? FreeTicketType { get; set; }
}
