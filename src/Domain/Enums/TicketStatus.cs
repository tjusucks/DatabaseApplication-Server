
namespace DbApp.Domain.Enums{
    public enum TicketStatus
    {
        Issued,    // 已发行
        Used,      // 已使用
        Expired,   // 已过期
        Refunded,  // 已退款
        Cancelled  // 已取消
    }
}