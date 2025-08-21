using DbApp.Domain.Entities.UserSystem;

namespace DbApp.Domain.Entities.TicketingSystem;

public class PriceHistory
{
    public int PriceHistoryId { get; set; }
    public int TicketTypeId { get; set; }
    public int? PriceRuleId { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public DateTime ChangeDatetime { get; set; }
    public int EmployeeId { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TicketType TicketType { get; set; } = null!;
    public PriceRule? PriceRule { get; set; }
    public Employee Employee { get; set; } = null!;
}
