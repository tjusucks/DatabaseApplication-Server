namespace DbApp.Domain.Entities;
using DbApp.Domain.Enums;

/// <summary>
/// 员工绩效考核记录
/// </summary>
public class EmployeeReview
{
    /// <summary>
    /// 绩效ID
    /// </summary>
    public int ReviewId { get; set; }

    /// <summary>
    /// 员工ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// 考核周期
    /// </summary>
    public string Period { get; set; } = null!;

    /// <summary>
    /// 绩效得分
    /// </summary>
    public decimal Score { get; set; }

    /// <summary>
    /// 评级
    /// </summary>
    public EvaluationLevel? EvaluationLevel { get; set; }

    /// <summary>
    /// 评估人ID
    /// </summary>
    public int? EvaluatorId { get; set; }

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
    public Employee? Evaluator { get; set; }
}
