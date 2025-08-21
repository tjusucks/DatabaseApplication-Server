namespace DbApp.Domain.Entities.TicketingSystem;

using System.ComponentModel.DataAnnotations;

public class ReservationItem
{
    public int ItemId { get; set; }
    public int ReservationId { get; set; }
    public int TicketTypeId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
    public int? AppliedPriceRuleId { get; set; }
    public decimal DiscountAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal LineTotal { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public Reservation Reservation { get; set; } = null!;
    public TicketType TicketType { get; set; } = null!;
    public PriceRule? AppliedPriceRule { get; set; } = null!;

    // 一个预订项目会生成多张票 (Ticket)
    public ICollection<Ticket> Tickets { get; set; } = [];
}

