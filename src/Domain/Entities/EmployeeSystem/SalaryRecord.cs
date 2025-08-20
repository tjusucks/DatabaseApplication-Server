namespace DbApp.Domain.Entities;

/// <summary>
/// 员工薪资记录
/// </summary>
public class SalaryRecord
{
    /// <summary>
    /// 薪资ID
    /// </summary>
    public int SalaryRecordId { get; set; }

    /// <summary>
    /// 员工ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// 发放日期
    /// </summary>
    public DateTime PayDate { get; set; }

    /// <summary>
    /// 工资
    /// </summary>
    public decimal Salary { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public Employee Employee { get; set; } = null!;
}
