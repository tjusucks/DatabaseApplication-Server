using DbApp.Domain.Enums;
using System.Collections.Generic;

namespace DbApp.Domain.Entities.TicketRelated
{
    public class TicketType
    {
        public int TicketTypeId { get; set; }
        public string TypeName { get; set; }
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public ApplicableCrowd ApplicableCrowd { get; set; }
        public string? RulesText { get; set; }
        public int? MaxSaleLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 导航属性：一个票种可以对应多个票
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        // 添加这个集合，表示一个票种可以参与多个活动（通过关联表）
        public ICollection<PromotionTicketType> ApplicablePromotions { get; set; } = new List<PromotionTicketType>();

        // 在这里添加下面这行代码
        public ICollection<PriceRule> PriceRules { get; set; } = new List<PriceRule>();
    }
}