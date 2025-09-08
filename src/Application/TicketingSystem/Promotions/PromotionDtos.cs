using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Application.TicketingSystem.Promotions;

public class PromotionDto
{
    public int PromotionId { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public PromotionType PromotionType { get; set; }
    public string? Description { get; set; }
    public DateTime StartDatetime { get; set; }
    public DateTime EndDatetime { get; set; }
    public int? UsageLimitPerUser { get; set; }
    public int? TotalUsageLimit { get; set; }
    public int CurrentUsageCount { get; set; }
    public int DisplayPriority { get; set; }
    public bool AppliesToAllTickets { get; set; }
    public bool IsActive { get; set; }
    public bool IsCombinable { get; set; }
    public int EmployeeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
