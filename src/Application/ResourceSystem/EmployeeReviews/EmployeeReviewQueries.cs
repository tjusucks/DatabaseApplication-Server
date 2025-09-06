using System.Collections.Generic;
using DbApp.Domain.Entities.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.EmployeeReviews
{
    // 查询命令
    public record GetEmployeeReviewByIdQuery(int ReviewId) : IRequest<EmployeeReviewDto?>;
    public record GetAllEmployeeReviewsQuery : IRequest<List<EmployeeReviewDto>>;
    public record GetEmployeeReviewsByEmployeeQuery(int EmployeeId) : IRequest<List<EmployeeReviewDto>>;
    public record GetEmployeeReviewsByPeriodQuery(string Period) : IRequest<List<EmployeeReviewDto>>;
    public record GetEmployeeReviewsByEvaluatorQuery(int EvaluatorId) : IRequest<List<EmployeeReviewDto>>;
    public record GetEmployeeReviewByEmployeeAndPeriodQuery(int EmployeeId, string Period) : IRequest<EmployeeReviewDto?>;
    public record GetEmployeeReviewStatisticsQuery(int EmployeeId, int Year, int? Month = null, int? Quarter = null) : IRequest<EmployeeReviewStatistics>;

    public class EmployeeReviewStatistics
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public int? Month { get; set; }
        public int? Quarter { get; set; }
        public decimal TotalScore { get; set; }
        public int ReviewCount { get; set; }
        public decimal AverageScore { get; set; }
    }
}
