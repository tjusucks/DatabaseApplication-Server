namespace DbApp.Domain.Entities.TicketingSystem;

public class PromotionTicketType
{
    // 外键和复合主键的一部分
    public int PromotionId { get; set; }
    // 外键和复合主键的一部分
    public int TicketTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    // 导航属性，指向关联的实体
    public Promotion Promotion { get; set; } = null!;
    public TicketType TicketType { get; set; } = null!;
}

