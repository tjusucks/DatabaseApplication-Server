using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace db.Models;

/// <summary>
/// 核心用户表，存储所有系统用户的基本信息
/// </summary>
[Table("USERS")]
[Index("CreatedAt", Name = "USERS_CREATED_AT_IDX")]
[Index("Email", Name = "USERS_EMAIL_UQ", IsUnique = true)]
[Index("RoleId", Name = "USERS_ROLE_ID_IDX")]
[Index("Username", Name = "USERS_USERNAME_UQ", IsUnique = true)]
public partial class User
{
    /// <summary>
    /// 用户唯一标识，主键
    /// </summary>
    [Key]
    [Column("USER_ID")]
    [Precision(10)]
    public int UserId { get; set; }

    /// <summary>
    /// 用户名，唯一且非空
    /// </summary>
    [Column("USERNAME")]
    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    /// <summary>
    /// 密码哈希值，使用安全算法加密存储
    /// </summary>
    [Column("PASSWORD_HASH")]
    [StringLength(256)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// 电子邮箱，唯一且非空
    /// </summary>
    [Column("EMAIL")]
    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    /// <summary>
    /// 用户显示名称，用于界面展示
    /// </summary>
    [Column("DISPLAY_NAME")]
    [StringLength(50)]
    [Unicode(false)]
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// 电话号码，可为空
    /// </summary>
    [Column("PHONE_NUMBER")]
    [StringLength(20)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 出生日期，时间戳格式
    /// </summary>
    [Column("BIRTH_DATE")]
    [Precision(0)]
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 性别，仅允许 male 或 female
    /// </summary>
    [Column("GENDER")]
    [StringLength(30)]
    [Unicode(false)]
    public string? Gender { get; set; }

    /// <summary>
    /// 用户注册时间，默认为系统时间
    /// </summary>
    [Column("REGISTER_TIME")]
    [Precision(0)]
    public DateTime RegisterTime { get; set; }

    /// <summary>
    /// 权限等级，0-9 表示不同权限级别，默认为0
    /// </summary>
    [Required]
    [Column("PERMISSION_LEVEL", TypeName = "NUMBER(1)")]
    public bool? PermissionLevel { get; set; }

    /// <summary>
    /// 记录创建时间，默认为系统时间
    /// </summary>
    [Column("CREATED_AT")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 记录最后更新时间，默认为系统时间
    /// </summary>
    [Column("UPDATED_AT")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 外键，关联角色表 role_id
    /// </summary>
    [Column("ROLE_ID")]
    [Precision(10)]
    public int? RoleId { get; set; }

    [InverseProperty("Visitor")]
    public virtual Blacklist? Blacklist { get; set; }

    [InverseProperty("EmployeeNavigation")]
    public virtual Employee? Employee { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role? Role { get; set; }

    [InverseProperty("VisitorNavigation")]
    public virtual Visitor? Visitor { get; set; }
}
