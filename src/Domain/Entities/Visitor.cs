using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace db.Models;

/// <summary>
/// 访客信息扩展表，补充核心用户中的游客相关信息
/// </summary>
[Table("VISITORS")]
[Index("Height", Name = "VISITORS_HEIGHT_IDX")]
[Index("IsBlacklisted", Name = "VISITORS_IS_BLACKLISTED_IDX")]
[Index("VisitorType", Name = "VISITORS_VISITOR_TYPE_IDX")]
public partial class Visitor
{
    /// <summary>
    /// 访客ID，同时也是用户ID，主键且外键
    /// </summary>
    [Key]
    [Column("VISITOR_ID")]
    [Precision(10)]
    public int VisitorId { get; set; }

    /// <summary>
    /// 访客类型：regular普通游客，member会员
    /// </summary>
    [Column("VISITOR_TYPE")]
    [StringLength(30)]
    [Unicode(false)]
    public string VisitorType { get; set; } = null!;

    /// <summary>
    /// 积分，可用于兑换或升级
    /// </summary>
    [Column("POINTS")]
    [Precision(10)]
    public int? Points { get; set; }

    /// <summary>
    /// 会员等级，如：bronze, silver, gold 等
    /// </summary>
    [Column("MEMBER_LEVEL")]
    [StringLength(30)]
    [Unicode(false)]
    public string? MemberLevel { get; set; }

    /// <summary>
    /// 成为会员的起始时间
    /// </summary>
    [Column("MEMBER_SINCE")]
    [Precision(0)]
    public DateTime? MemberSince { get; set; }

    /// <summary>
    /// 是否在黑名单中：0否，1是
    /// </summary>
    [Required]
    [Column("IS_BLACKLISTED", TypeName = "NUMBER(1)")]
    public bool? IsBlacklisted { get; set; }

    /// <summary>
    /// 身高（单位：cm），用于游乐设施限制判断
    /// </summary>
    [Column("HEIGHT")]
    [Precision(4)]
    public byte Height { get; set; }

    [InverseProperty("UsedByNavigation")]
    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    [InverseProperty("Visitor")]
    public virtual ICollection<EntryRecord> EntryRecords { get; set; } = new List<EntryRecord>();

    [InverseProperty("Visitor")]
    public virtual ICollection<RefundRecord> RefundRecords { get; set; } = new List<RefundRecord>();

    [InverseProperty("Visitor")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [InverseProperty("Visitor")]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    [ForeignKey("VisitorId")]
    [InverseProperty("Visitor")]
    public virtual User VisitorNavigation { get; set; } = null!;
}
