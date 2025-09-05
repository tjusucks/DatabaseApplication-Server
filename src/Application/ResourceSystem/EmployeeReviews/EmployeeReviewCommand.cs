using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.EmployeeReviews
{
    // 基础CRUD命令
    public record CreateEmployeeReviewCommand(
        int EmployeeId,
        string Period,
        decimal Score,
        EvaluationLevel? EvaluationLevel,
        int? EvaluatorId) : IRequest<int>;

    public record UpdateEmployeeReviewCommand(
        int ReviewId,
        decimal Score,
        EvaluationLevel? EvaluationLevel,
        int? EvaluatorId) : IRequest;

    public record DeleteEmployeeReviewCommand(int ReviewId) : IRequest;

    // 考勤扣分命令
    public record CreateAttendanceDeductionCommand(
        int EmployeeId,
        string Period,
        int? EvaluatorId) : IRequest<int>;
}
