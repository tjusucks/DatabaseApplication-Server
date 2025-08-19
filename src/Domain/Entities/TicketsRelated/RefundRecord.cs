using DbApp.Domain.Enums;
namespace DbApp.Domain.Entities.TicketRelated
{
    public class RefundRecord
    {
        public int RefundId { get; set; }
        public int TicketId { get; set; } // 这是一个一对一关系的关键
        public int VisitorId { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime RefundTime { get; set; }
        public string? RefundReason { get; set; }
        public RefundStatus RefundStatus { get; set; }
        public int? ProcessorId { get; set; }
        public string? ProcessingNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 导航属性
        public Ticket Ticket { get; set; }
        public Visitor Visitor { get; set; }
        public Employee? Processor { get; set; }
    }
}