using DbApp.Domain.Enums;
using System.Collections.Generic;
namespace DbApp.Domain.Entities.TicketRelated
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int VisitorId { get; set; }
        public DateTime ReservationTime { get; set; }
        public DateTime VisitDate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ReservationStatus Status { get; set; }
        public string? PaymentMethod { get; set; }
        public int? PromotionId { get; set; }
        public string? SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 导航属性
        public Visitor Visitor { get; set; }
        public Promotion? Promotion { get; set; }

        // 一个预订包含多个预订项目 (ReservationItem)
        public ICollection<ReservationItem> ReservationItems { get; set; } = new List<ReservationItem>();
    }
}