using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Domain.Entities.TicketingSystem;

public class Ticket
{
    public int TicketId { get; set; }
    public int ReservationItemId { get; set; }
    public int TicketTypeId { get; set; }
    public int? VisitorId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime? UsedTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    // 导航属性
    public RefundRecord? RefundRecord { get; set; }
    public ReservationItem ReservationItem { get; set; } = null!;
    public TicketType TicketType { get; set; } = null!;
    public Visitor? Visitor { get; set; }
}
