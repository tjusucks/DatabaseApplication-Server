using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Application.TicketingSystem.PromotionConditions;

public class PromotionConditionDto
{
    public int ConditionId { get; set; }
    public int PromotionId { get; set; }
    public string ConditionName { get; set; } = string.Empty;
    public ConditionType ConditionType { get; set; }
    public int? TicketTypeId { get; set; }
    public int? MinQuantity { get; set; }
    public decimal? MinAmount { get; set; }
    public string? VisitorType { get; set; }
    public string? MemberLevel { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? DayOfWeek { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
