
namespace DbApp.Domain.Entities.TicketRelated
{
    public class PriceHistory
    {
        public int PriceHistoryId { get; set; }
        public int TicketTypeId { get; set; }
        public int? PriceRuleId { get; set; } // 注意: 表结构中没有 NOT NULL，所以是可空的
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangeDatetime { get; set; }
        public int EmployeeId { get; set; }
        public string? Reason { get; set; } // 可空
        public DateTime CreatedAt { get; set; }

        // 导航属性
        public TicketType TicketType { get; set; }
        public PriceRule? PriceRule { get; set; } // 对应可空外键
        public Employee Employee { get; set; }
    }
}