using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Domain.Entities.TicketingSystem;

public class RefundRecord
{
    public int RefundId { get; set; }
    public int TicketId { get; set; }
    public int VisitorId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal RefundAmount { get; set; }
    public DateTime RefundTime { get; set; }
    public string? RefundReason { get; set; }
    public RefundStatus RefundStatus { get; set; }
    public int? ProcessorId { get; set; }
    public string? ProcessingNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public Ticket Ticket { get; set; } = null!;
    public Visitor Visitor { get; set; } = null!;
    public Employee? Processor { get; set; } = null!;
}
