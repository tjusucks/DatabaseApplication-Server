namespace DbApp.Domain.Enums
{
    public enum RefundStatus
    {
        Pending,   // 'pending' - 待处理
        Approved,  // 'approved' - 已批准
        Rejected,  // 'rejected' - 已拒绝
        Completed  // 'completed' - 已完成
    }
}