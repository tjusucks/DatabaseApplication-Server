
namespace DbApp.Domain.Enums
{
    public enum ConditionType
    {
        MinQuantity,     // 'MIN_QUANTITY' - 最小数量
        MinAmount,       // 'MIN_AMOUNT' - 最小金额
        SpecificTicket,  // 'SPECIFIC_TICKET' - 特定票种
        VisitorType,     // 'VISITOR_TYPE' - 访客类型
        VisitDate,       // 'VISIT_DATE' - 访问日期
        DayOfWeek,       // 'DAY_OF_WEEK' - 星期几
        MemberLevel      // 'MEMBER_LEVEL' - 会员等级
    }
}