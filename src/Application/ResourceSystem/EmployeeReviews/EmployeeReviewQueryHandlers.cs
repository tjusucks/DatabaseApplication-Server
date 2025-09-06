using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.EmployeeReviews
{
    public class GetEmployeeReviewByIdQueryHandler : IRequestHandler<GetEmployeeReviewByIdQuery, EmployeeReviewDto?>
    {
        private readonly IEmployeeReviewRepository _employeeReviewRepository;

        public GetEmployeeReviewByIdQueryHandler(IEmployeeReviewRepository employeeReviewRepository)
        {
            _employeeReviewRepository = employeeReviewRepository;
        }

        public async Task<EmployeeReviewDto?> Handle(GetEmployeeReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var review = await _employeeReviewRepository.GetByIdAsync(request.ReviewId);
            if (review == null) return null;

            return new EmployeeReviewDto
            {
                ReviewId = review.ReviewId,
                EmployeeId = review.EmployeeId,
                Period = review.Period,
                Score = review.Score,
                EvaluationLevel = review.EvaluationLevel?.ToString(),
                EvaluatorId = review.EvaluatorId,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                Employee = new EmployeeSimpleDto
                {
                    EmployeeId = review.Employee.EmployeeId,
                    StaffNumber = review.Employee.StaffNumber,
                    Position = review.Employee.Position,
                    DepartmentName = review.Employee?.DepartmentName ?? string.Empty
                }
            };
        }
    }

    public class GetAllEmployeeReviewsQueryHandler : IRequestHandler<GetAllEmployeeReviewsQuery, List<EmployeeReviewDto>>
    {
        private readonly IEmployeeReviewRepository _employeeReviewRepository;

        public GetAllEmployeeReviewsQueryHandler(IEmployeeReviewRepository employeeReviewRepository)
        {
            _employeeReviewRepository = employeeReviewRepository;
        }

        public async Task<List<EmployeeReviewDto>> Handle(GetAllEmployeeReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _employeeReviewRepository.GetAllAsync();

            return reviews.Select(review => new EmployeeReviewDto
            {
                ReviewId = review.ReviewId,
                EmployeeId = review.EmployeeId,
                Period = review.Period,
                Score = review.Score,
                EvaluationLevel = review.EvaluationLevel?.ToString(),
                EvaluatorId = review.EvaluatorId,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                Employee = new EmployeeSimpleDto
                {
                    EmployeeId = review.Employee.EmployeeId,
                    StaffNumber = review.Employee.StaffNumber,
                    Position = review.Employee.Position,
                    DepartmentName = review.Employee?.DepartmentName ?? string.Empty
                }
            }).ToList();
        }
    }

    public class GetEmployeeReviewsByEmployeeQueryHandler : IRequestHandler<GetEmployeeReviewsByEmployeeQuery, List<EmployeeReviewDto>>
    {
        private readonly IEmployeeReviewRepository _employeeReviewRepository;

        public GetEmployeeReviewsByEmployeeQueryHandler(IEmployeeReviewRepository employeeReviewRepository)
        {
            _employeeReviewRepository = employeeReviewRepository;
        }

        public async Task<List<EmployeeReviewDto>> Handle(GetEmployeeReviewsByEmployeeQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _employeeReviewRepository.GetByEmployeeAsync(request.EmployeeId);

            return reviews.Select(review => new EmployeeReviewDto
            {
                ReviewId = review.ReviewId,
                EmployeeId = review.EmployeeId,
                Period = review.Period,
                Score = review.Score,
                EvaluationLevel = review.EvaluationLevel?.ToString(),
                EvaluatorId = review.EvaluatorId,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                Employee = new EmployeeSimpleDto
                {
                    EmployeeId = review.Employee.EmployeeId,
                    StaffNumber = review.Employee.StaffNumber,
                    Position = review.Employee.Position,
                    DepartmentName = review.Employee?.DepartmentName ?? string.Empty
                }
            }).ToList();
        }
    }

    public class GetEmployeeReviewsByPeriodQueryHandler : IRequestHandler<GetEmployeeReviewsByPeriodQuery, List<EmployeeReviewDto>>
    {
        private readonly IEmployeeReviewRepository _employeeReviewRepository;

        public GetEmployeeReviewsByPeriodQueryHandler(IEmployeeReviewRepository employeeReviewRepository)
        {
            _employeeReviewRepository = employeeReviewRepository;
        }

        public async Task<List<EmployeeReviewDto>> Handle(GetEmployeeReviewsByPeriodQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _employeeReviewRepository.GetByPeriodAsync(request.Period);

            return reviews.Select(review => new EmployeeReviewDto
            {
                ReviewId = review.ReviewId,
                EmployeeId = review.EmployeeId,
                Period = review.Period,
                Score = review.Score,
                EvaluationLevel = review.EvaluationLevel?.ToString(),
                EvaluatorId = review.EvaluatorId,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                Employee = new EmployeeSimpleDto
                {
                    EmployeeId = review.Employee.EmployeeId,
                    StaffNumber = review.Employee.StaffNumber,
                    Position = review.Employee.Position,
                    DepartmentName = review.Employee?.DepartmentName ?? string.Empty
                }
            }).ToList();
        }
    }

    public class GetEmployeeReviewsByEvaluatorQueryHandler : IRequestHandler<GetEmployeeReviewsByEvaluatorQuery, List<EmployeeReviewDto>>
    {
        private readonly IEmployeeReviewRepository _employeeReviewRepository;

        public GetEmployeeReviewsByEvaluatorQueryHandler(IEmployeeReviewRepository employeeReviewRepository)
        {
            _employeeReviewRepository = employeeReviewRepository;
        }

        public async Task<List<EmployeeReviewDto>> Handle(GetEmployeeReviewsByEvaluatorQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _employeeReviewRepository.GetByEvaluatorAsync(request.EvaluatorId);

            return reviews.Select(review => new EmployeeReviewDto
            {
                ReviewId = review.ReviewId,
                EmployeeId = review.EmployeeId,
                Period = review.Period,
                Score = review.Score,
                EvaluationLevel = review.EvaluationLevel?.ToString(),
                EvaluatorId = review.EvaluatorId,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                Employee = new EmployeeSimpleDto
                {
                    EmployeeId = review.Employee.EmployeeId,
                    StaffNumber = review.Employee.StaffNumber,
                    Position = review.Employee.Position,
                    DepartmentName = review.Employee?.DepartmentName ?? string.Empty
                }
            }).ToList();
        }
    }

    public class GetEmployeeReviewByEmployeeAndPeriodQueryHandler : IRequestHandler<GetEmployeeReviewByEmployeeAndPeriodQuery, EmployeeReviewDto?>
    {
        private readonly IEmployeeReviewRepository _employeeReviewRepository;

        public GetEmployeeReviewByEmployeeAndPeriodQueryHandler(IEmployeeReviewRepository employeeReviewRepository)
        {
            _employeeReviewRepository = employeeReviewRepository;
        }

        public async Task<EmployeeReviewDto?> Handle(GetEmployeeReviewByEmployeeAndPeriodQuery request, CancellationToken cancellationToken)
        {
            var review = await _employeeReviewRepository.GetByEmployeeAndPeriodAsync(request.EmployeeId, request.Period);
            if (review == null) return null;

            return new EmployeeReviewDto
            {
                ReviewId = review.ReviewId,
                EmployeeId = review.EmployeeId,
                Period = review.Period,
                Score = review.Score,
                EvaluationLevel = review.EvaluationLevel?.ToString(),
                EvaluatorId = review.EvaluatorId,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                Employee = new EmployeeSimpleDto
                {
                    EmployeeId = review.Employee.EmployeeId,
                    StaffNumber = review.Employee.StaffNumber,
                    Position = review.Employee.Position,
                    DepartmentName = review.Employee?.DepartmentName ?? string.Empty
                }
            };
        }
    }

    public class GetEmployeeReviewStatisticsQueryHandler : IRequestHandler<GetEmployeeReviewStatisticsQuery, EmployeeReviewStatistics>
    {
        private readonly IEmployeeReviewRepository _employeeReviewRepository;

        public GetEmployeeReviewStatisticsQueryHandler(IEmployeeReviewRepository employeeReviewRepository)
        {
            _employeeReviewRepository = employeeReviewRepository;
        }

        public async Task<EmployeeReviewStatistics> Handle(GetEmployeeReviewStatisticsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _employeeReviewRepository.GetByEmployeeAsync(request.EmployeeId);

            // 根据查询类型筛选数据
            var filteredReviews = reviews.Where(r =>
            {
                if (!int.TryParse(r.Period.Substring(0, 4), out int reviewYear))
                    return false;

                if (reviewYear != request.Year)
                    return false;

                if (request.Month.HasValue)
                {
                    // 按月查询
                    if (r.Period.Length >= 7 && int.TryParse(r.Period.Substring(5, 2), out int reviewMonth))
                    {
                        return reviewMonth == request.Month.Value;
                    }
                    return false;
                }
                else if (request.Quarter.HasValue)
                {
                    // 按季度查询
                    if (r.Period.Length >= 7 && int.TryParse(r.Period.Substring(5, 2), out int reviewMonth))
                    {
                        int reviewQuarter = (reviewMonth - 1) / 3 + 1;
                        return reviewQuarter == request.Quarter.Value;
                    }
                    return false;
                }
                // 年度查询
                return true;
            }).ToList();

            var totalScore = filteredReviews.Sum(r => r.Score);
            var reviewCount = filteredReviews.Count;
            var averageScore = reviewCount > 0 ? totalScore / reviewCount : 0;

            return new EmployeeReviewStatistics
            {
                EmployeeId = request.EmployeeId,
                Year = request.Year,
                Month = request.Month,
                Quarter = request.Quarter,
                TotalScore = totalScore,
                ReviewCount = reviewCount,
                AverageScore = averageScore
            };
        }
    }
}
