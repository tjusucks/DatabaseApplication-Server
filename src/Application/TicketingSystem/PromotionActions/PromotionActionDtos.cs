using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Application.TicketingSystem.PromotionActions;

public class PromotionActionDto
{
    public int ActionId { get; set; }
    public int PromotionId { get; set; }
    public string ActionName { get; set; } = string.Empty;
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
}
