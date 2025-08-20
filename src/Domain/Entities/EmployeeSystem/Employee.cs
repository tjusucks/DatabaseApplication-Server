namespace DbApp.Domain.Entities;

/// <summary>
/// 员工表，存储园区所有员工信息
/// </summary>
public class Employee
{
    /// <summary>
    /// 员工ID，同时也是用户ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// 员工工号，唯一标识
    /// </summary>
    public string StaffNumber { get; set; } = null!;

    /// <summary>
    /// 职位名称，如：园长、技术员等
    /// </summary>
    public string Position { get; set; } = null!;

    /// <summary>
    /// 所属部门名称
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// 工作人员类型：regular普通, inspector巡检员, mechanic维修员, manager管理员
    /// </summary>
    public string? StaffType { get; set; }

    /// <summary>
    /// 所属团队ID
    /// </summary>
    public int? TeamId { get; set; }

    /// <summary>
    /// 入职日期
    /// </summary>
    public DateTime HireDate { get; set; }

    /// <summary>
    /// 雇佣状态：active在职, resigned已离职, on_leave请假中
    /// </summary>
    public string EmploymentStatus { get; set; } = null!;

    /// <summary>
    /// 直属经理的员工ID
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// 持有的专业认证信息
    /// </summary>
    public string? Certification { get; set; }

    /// <summary>
    /// 负责区域或设施范围
    /// </summary>
    public string? ResponsibilityArea { get; set; }

    /// <summary>
    /// 记录创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 记录最后更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public User EmployeeNavigation { get; set; } = null!;
    public Employee? Manager { get; set; }
    public StaffTeam? Team { get; set; }

    // 集合导航属性
    public ICollection<AmusementRide> AmusementRides { get; set; } = new List<AmusementRide>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Employee> InverseManager { get; set; } = new List<Employee>();
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
    public ICollection<PriceRule> PriceRules { get; set; } = new List<PriceRule>();
    public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    public ICollection<RefundRecord> RefundRecords { get; set; } = new List<RefundRecord>();
    public ICollection<SalaryRecord> SalaryRecords { get; set; } = new List<SalaryRecord>();
    public ICollection<StaffTeam> StaffTeams { get; set; } = new List<StaffTeam>();
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    public ICollection<EmployeeReview> EmployeeReviewEmployees { get; set; } = new List<EmployeeReview>();
    public ICollection<EmployeeReview> EmployeeReviewEvaluators { get; set; } = new List<EmployeeReview>();
    public ICollection<FinancialRecord> FinancialRecordApprovedByNavigations { get; set; } = new List<FinancialRecord>();
    public ICollection<FinancialRecord> FinancialRecordResponsibleEmployees { get; set; } = new List<FinancialRecord>();
}
