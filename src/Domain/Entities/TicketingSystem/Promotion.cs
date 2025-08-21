using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Domain.Entities.TicketingSystem;

public class Promotion
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public Employee Employee { get; set; } = null!;
    public ICollection<PromotionTicketType> ApplicableTicketTypes { get; set; } = [];
    public ICollection<PromotionCondition> Conditions { get; set; } = [];
    public ICollection<PromotionAction> Actions { get; set; } = [];
    public ICollection<Coupon> Coupons { get; set; } = [];
}
