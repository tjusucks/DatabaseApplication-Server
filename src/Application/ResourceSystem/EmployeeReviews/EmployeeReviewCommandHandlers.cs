using System.Globalization;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.EmployeeReviews;

public class CreateEmployeeReviewCommandHandler(IEmployeeReviewRepository employeeReviewRepository) : IRequestHandler<CreateEmployeeReviewCommand, int>
{
    private readonly IEmployeeReviewRepository _employeeReviewRepository = employeeReviewRepository;

    public async Task<int> Handle(CreateEmployeeReviewCommand request, CancellationToken cancellationToken)
    {
        var review = new EmployeeReview
        {
            EmployeeId = request.EmployeeId,
            Period = request.Period,
            Score = request.Score,
            EvaluationLevel = request.EvaluationLevel,
            EvaluatorId = request.EvaluatorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _employeeReviewRepository.CreateAsync(review);
    }
}

public class UpdateEmployeeReviewCommandHandler(IEmployeeReviewRepository employeeReviewRepository) : IRequestHandler<UpdateEmployeeReviewCommand>
{
    private readonly IEmployeeReviewRepository _employeeReviewRepository = employeeReviewRepository;

    public async Task Handle(UpdateEmployeeReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _employeeReviewRepository.GetByIdAsync(request.ReviewId)
            ?? throw new InvalidOperationException($"未找到ID为 {request.ReviewId} 的员工绩效记录");
        review.Score = request.Score;
        review.EvaluationLevel = request.EvaluationLevel;
        review.EvaluatorId = request.EvaluatorId;
        review.UpdatedAt = DateTime.UtcNow;

        await _employeeReviewRepository.UpdateAsync(review);
    }
}

public class DeleteEmployeeReviewCommandHandler(IEmployeeReviewRepository employeeReviewRepository) : IRequestHandler<DeleteEmployeeReviewCommand>
{
    private readonly IEmployeeReviewRepository _employeeReviewRepository = employeeReviewRepository;

    public async Task Handle(DeleteEmployeeReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _employeeReviewRepository.GetByIdAsync(request.ReviewId)
            ?? throw new InvalidOperationException($"未找到ID为 {request.ReviewId} 的员工绩效记录");
        await _employeeReviewRepository.DeleteAsync(review);
    }
}

// 考勤扣分命令处理程序
public class CreateAttendanceDeductionCommandHandler(
    IEmployeeReviewRepository employeeReviewRepository,
    IAttendanceRepository attendanceRepository) : IRequestHandler<CreateAttendanceDeductionCommand, int>
{
    private readonly IEmployeeReviewRepository _employeeReviewRepository = employeeReviewRepository;
    private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;

    public async Task<int> Handle(CreateAttendanceDeductionCommand request, CancellationToken cancellationToken)
    {
        // 解析考核周期
        var (startDate, endDate) = ParsePeriod(request.Period);

        // 检查员工在该周期内是否全勤
        var (_, lateDays, absentDays, _) =
            await _attendanceRepository.GetEmployeeStatsAsync(
                request.EmployeeId,
                startDate,
                endDate
            );

        // 如果员工全勤，则不需要创建扣分记录
        if (lateDays == 0 && absentDays == 0)
        {
            throw new InvalidOperationException("员工在该周期内全勤，无需创建考勤扣分记录");
        }

        // 计算扣分值 - 可以根据迟到/缺勤次数计算
        var deductionScore = CalculateDeductionScore(lateDays, absentDays);

        // 创建绩效扣分记录
        var deductionReview = new EmployeeReview
        {
            EmployeeId = request.EmployeeId,
            Period = request.Period,
            Score = deductionScore, // 扣分为负值或较低的分数
            EvaluationLevel = null, // 可以根据扣分情况设置等级
            EvaluatorId = request.EvaluatorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _employeeReviewRepository.CreateAsync(deductionReview);
    }

    private static (DateTime startDate, DateTime endDate) ParsePeriod(string period)
    {
        // 假设周期格式为 "yyyy-MM"，例如 "2025-08"
        if (DateTime.TryParseExact(period + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            var startDate = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Unspecified);
            var lastDay = DateTime.DaysInMonth(date.Year, date.Month);
            var endDate = new DateTime(date.Year, date.Month, lastDay, 0, 0, 0, DateTimeKind.Unspecified);
            return (startDate, endDate);
        }

        // 默认返回当前月
        var now = DateTime.Now;
        var defaultStartDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var defaultLastDay = DateTime.DaysInMonth(now.Year, now.Month);
        var defaultEndDate = new DateTime(now.Year, now.Month, defaultLastDay, 0, 0, 0, DateTimeKind.Unspecified);
        return (defaultStartDate, defaultEndDate);
    }

    private static decimal CalculateDeductionScore(int lateDays, int absentDays)
    {
        // 扣分规则：每次迟到扣1分，每次缺勤扣3分
        var totalDeduction = lateDays * 1 + absentDays * 3;
        // 限制最大扣分不超过20分，并将扣分表示为负数
        return -Math.Min(totalDeduction, 20);
    }
}
