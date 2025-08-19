using DbApp.Domain.Enums;  
namespace DbApp.Domain.Entities.TicketRelated{
    public class Ticket
    {
        // ticket_id: 主键
        public int TicketId { get; set; }

        // reservation_item_id: 外键
        public int ReservationItemId { get; set; }
        // 导航属性，用于 EF Core 建立关系
        public ReservationItem ReservationItem { get; set; }

        // ticket_type_id: 外键
        public int TicketTypeId { get; set; }
        // 导航属性
        public TicketType TicketType { get; set; }

        // visitor_id: 可空外键
        public int? VisitorId { get; set; }
        // 导航属性
        public Visitor? Visitor { get; set; }

        // serial_number: 唯一，非空
        public string SerialNumber { get; set; }

        // valid_from: 非空
        public DateTime ValidFrom { get; set; }

        // valid_to: 非空
        public DateTime ValidTo { get; set; }

        // status: 状态，使用枚举
        public TicketStatus Status { get; set; }

        // used_time: 可空
        public DateTime? UsedTime { get; set; }

        // created_at: 创建时间，通常由数据库生成
        public DateTime CreatedAt { get; set; }

        // updated_at: 更新时间，通常由数据库生成
        public DateTime UpdatedAt { get; set; }
        
        public RefundRecord? RefundRecord { get; set; }
    }
}