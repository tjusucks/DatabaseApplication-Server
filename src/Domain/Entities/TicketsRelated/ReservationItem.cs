
namespace DbApp.Domain.Entities.TicketRelated
{
    public class ReservationItem
    {
        public int ItemId { get; set; }
        public int ReservationId { get; set; }
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int? AppliedPriceRuleId { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal LineTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 导航属性
        public Reservation Reservation { get; set; }
        public TicketType TicketType { get; set; }
        public PriceRule? AppliedPriceRule { get; set; }

        // 一个预订项目会生成多张票 (Ticket)
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}