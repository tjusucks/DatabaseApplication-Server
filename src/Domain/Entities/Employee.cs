using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace db.Models;

/// <summary>
/// 员工表，存储园区所有员工信息，外键引用 users 表
/// </summary>
[Table("EMPLOYEES")]
[Index("DepartmentName", Name = "EMPLOYEES_DEPARTMENT_NAME_IDX")]
[Index("EmploymentStatus", Name = "EMPLOYEES_EMPLOYMENT_STATUS_IDX")]
[Index("ManagerId", Name = "EMPLOYEES_MANAGER_ID_IDX")]
[Index("StaffNumber", Name = "EMPLOYEES_STAFF_NUMBER_UQ", IsUnique = true)]
[Index("StaffType", Name = "EMPLOYEES_STAFF_TYPE_IDX")]
[Index("TeamId", Name = "EMPLOYEES_TEAM_ID_IDX")]
public partial class Employee
{
    /// <summary>
    /// 员工ID，同时也是用户ID，主键且外键
    /// </summary>
    [Key]
    [Column("EMPLOYEE_ID")]
    [Precision(10)]
    public int EmployeeId { get; set; }

    /// <summary>
    /// 员工工号，唯一标识
    /// </summary>
    [Column("STAFF_NUMBER")]
    [StringLength(20)]
    [Unicode(false)]
    public string StaffNumber { get; set; } = null!;

    /// <summary>
    /// 职位名称，如：园长、技术员等
    /// </summary>
    [Column("POSITION")]
    [StringLength(50)]
    [Unicode(false)]
    public string Position { get; set; } = null!;

    /// <summary>
    /// 所属部门名称
    /// </summary>
    [Column("DEPARTMENT_NAME")]
    [StringLength(50)]
    [Unicode(false)]
    public string? DepartmentName { get; set; }

    /// <summary>
    /// 工作人员类型：regular普通, inspector巡检员, mechanic维修员, manager管理员
    /// </summary>
    [Column("STAFF_TYPE")]
    [StringLength(30)]
    [Unicode(false)]
    public string? StaffType { get; set; }

    /// <summary>
    /// 所属团队ID，外键引用 staff_teams
    /// </summary>
    [Column("TEAM_ID")]
    [Precision(10)]
    public int? TeamId { get; set; }

    /// <summary>
    /// 入职日期，默认为系统时间
    /// </summary>
    [Column("HIRE_DATE")]
    [Precision(0)]
    public DateTime HireDate { get; set; }

    /// <summary>
    /// 雇佣状态：active在职, resigned已离职, on_leave请假中
    /// </summary>
    [Column("EMPLOYMENT_STATUS")]
    [StringLength(30)]
    [Unicode(false)]
    public string EmploymentStatus { get; set; } = null!;

    /// <summary>
    /// 直属经理的员工ID，自引用
    /// </summary>
    [Column("MANAGER_ID")]
    [Precision(10)]
    public int? ManagerId { get; set; }

    /// <summary>
    /// 持有的专业认证信息
    /// </summary>
    [Column("CERTIFICATION")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Certification { get; set; }

    /// <summary>
    /// 负责区域或设施范围
    /// </summary>
    [Column("RESPONSIBILITY_AREA")]
    [StringLength(100)]
    [Unicode(false)]
    public string? ResponsibilityArea { get; set; }

    /// <summary>
    /// 记录创建时间
    /// </summary>
    [Column("CREATED_AT")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 记录最后更新时间
    /// </summary>
    [Column("UPDATED_AT")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Manager")]
    public virtual ICollection<AmusementRide> AmusementRides { get; set; } = new List<AmusementRide>();

    [InverseProperty("Employee")]
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    [ForeignKey("EmployeeId")]
    [InverseProperty("Employee")]
    public virtual User EmployeeNavigation { get; set; } = null!;

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeReview> EmployeeReviewEmployees { get; set; } = new List<EmployeeReview>();

    [InverseProperty("Evaluator")]
    public virtual ICollection<EmployeeReview> EmployeeReviewEvaluators { get; set; } = new List<EmployeeReview>();

    [InverseProperty("ApprovedByNavigation")]
    public virtual ICollection<FinancialRecord> FinancialRecordApprovedByNavigations { get; set; } = new List<FinancialRecord>();

    [InverseProperty("ResponsibleEmployee")]
    public virtual ICollection<FinancialRecord> FinancialRecordResponsibleEmployees { get; set; } = new List<FinancialRecord>();

    [InverseProperty("Manager")]
    public virtual ICollection<Employee> InverseManager { get; set; } = new List<Employee>();

    [InverseProperty("Manager")]
    public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();

    [ForeignKey("ManagerId")]
    [InverseProperty("InverseManager")]
    public virtual Employee? Manager { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<PriceRule> PriceRules { get; set; } = new List<PriceRule>();

    [InverseProperty("Employee")]
    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    [InverseProperty("Processor")]
    public virtual ICollection<RefundRecord> RefundRecords { get; set; } = new List<RefundRecord>();

    [InverseProperty("Employee")]
    public virtual ICollection<SalaryRecord> SalaryRecords { get; set; } = new List<SalaryRecord>();

    [InverseProperty("Leader")]
    public virtual ICollection<StaffTeam> StaffTeams { get; set; } = new List<StaffTeam>();

    [ForeignKey("TeamId")]
    [InverseProperty("Employees")]
    public virtual StaffTeam? Team { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
