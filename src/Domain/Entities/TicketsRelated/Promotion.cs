// 位置: Domain/Entities/Promotion.cs

using DbApp.Domain.Enums;
using System.Collections.Generic;


namespace DbApp.Domain.Entities.TicketRelated
{
    public class Promotion
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public PromotionType PromotionType { get; set; }
        public string? Description { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
        public int? UsageLimitPerUser { get; set; }
        public int? TotalUsageLimit { get; set; }
        public int CurrentUsageCount { get; set; }
        public int DisplayPriority { get; set; }
        public bool AppliesToAllTickets { get; set; }
        public bool IsActive { get; set; }
        public bool IsCombinable { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 导航属性
        public Employee Employee { get; set; }
        public ICollection<PromotionTicketType> ApplicableTicketTypes { get; set; } = new List<PromotionTicketType>();
        // 新增：一个活动可以有多个触发条件
        public ICollection<PromotionCondition> Conditions { get; set; } = new List<PromotionCondition>();

        // 新增：一个活动可以包含多个动作
        public ICollection<PromotionAction> Actions { get; set; } = new List<PromotionAction>();

         // 新增：一个活动可以生成多张优惠券
        public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
    }
}