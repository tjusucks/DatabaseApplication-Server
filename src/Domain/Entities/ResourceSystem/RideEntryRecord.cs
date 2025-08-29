namespace DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Entities.TicketingSystem;
  
public class RideEntryRecord    
{    
    /// <summary>  
    /// 记录ID，主键  
    /// </summary>  
    public int RideEntryRecordId { get; set; }  
      
    /// <summary>  
    /// 游乐设施ID，外键  
    /// </summary>  
    public int RideId { get; set; }  
      
    /// <summary>  
    /// 游客ID，外键  
    /// </summary>  
    public int VisitorId { get; set; }  
      
    /// <summary>  
    /// 进入时间  
    /// </summary>  
    public DateTime EntryTime { get; set; } = DateTime.UtcNow;  
      
    /// <summary>  
    /// 退出时间（可选）  
    /// </summary>  
    public DateTime? ExitTime { get; set; }  
      
    /// <summary>  
    /// 关联门票ID（可选）  
    /// </summary>  
    public int? TicketId { get; set; }  
      
    /// <summary>  
    /// 创建时间  
    /// </summary>  
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
      
    /// <summary>  
    /// 更新时间  
    /// </summary>  
    public DateTime? UpdatedAt { get; set; }  
      
    // 导航属性  
    public AmusementRide Ride { get; set; } = null!;  
    public Visitor Visitor { get; set; } = null!;  
    public Ticket? Ticket { get; set; }    
}