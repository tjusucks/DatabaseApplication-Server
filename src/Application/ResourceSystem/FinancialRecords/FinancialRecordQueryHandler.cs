using AutoMapper;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.FinancialRecords;

public class FinancialRecordQueryHandler(
    IFinancialRecordRepository financialRecordRepository,
    IMapper mapper) :
    IRequestHandler<SearchFinancialRecordQuery, FinancialRecordResult>,
    IRequestHandler<GetFinancialRecordByIdQuery, FinancialRecordDetailDto?>,
    IRequestHandler<GetFinancialStatsQuery, FinancialStatsDto>,
    IRequestHandler<GetGroupedFinancialStatsQuery, List<GroupedFinancialStatsDto>>,
    IRequestHandler<GetIncomeExpenseOverviewQuery, IncomeExpenseOverviewDto>,
    IRequestHandler<GetFinancialRecordsByTypeQuery, FinancialRecordResult>
{
    private readonly IFinancialRecordRepository _financialRecordRepository = financialRecordRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<FinancialRecordResult> Handle(SearchFinancialRecordQuery request, CancellationToken cancellationToken)
    {
        var records = await _financialRecordRepository.SearchAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.TransactionType,
            request.PaymentMethod,
            request.ResponsibleEmployeeId,
            request.ApprovedById,
            request.MinAmount,
            request.MaxAmount,
            request.SortBy,
            request.Descending,
            request.Page,
            request.PageSize);

        var totalCount = await _financialRecordRepository.CountAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.TransactionType,
            request.PaymentMethod,
            request.ResponsibleEmployeeId,
            request.ApprovedById,
            request.MinAmount,
            request.MaxAmount);

        var summaryDtos = _mapper.Map<List<FinancialRecordSummaryDto>>(records);

        return new FinancialRecordResult
        {
            FinancialRecords = summaryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<FinancialRecordDetailDto?> Handle(GetFinancialRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var record = await _financialRecordRepository.GetByIdAsync(request.RecordId);
        return record == null ? null : _mapper.Map<FinancialRecordDetailDto>(record);
    }

    public async Task<FinancialStatsDto> Handle(GetFinancialStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await _financialRecordRepository.GetStatsAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.TransactionType,
            request.PaymentMethod,
            request.ResponsibleEmployeeId,
            request.ApprovedById,
            request.MinAmount,
            request.MaxAmount);

        return _mapper.Map<FinancialStatsDto>(stats);
    }

    public async Task<List<GroupedFinancialStatsDto>> Handle(GetGroupedFinancialStatsQuery request, CancellationToken cancellationToken)
    {
        var groupedStats = await _financialRecordRepository.GetGroupedStatsAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.TransactionType,
            request.PaymentMethod,
            request.ResponsibleEmployeeId,
            request.ApprovedById,
            request.MinAmount,
            request.MaxAmount,
            request.GroupBy,
            request.SortBy,
            request.Descending);

        return _mapper.Map<List<GroupedFinancialStatsDto>>(groupedStats);
    }

    public async Task<IncomeExpenseOverviewDto> Handle(GetIncomeExpenseOverviewQuery request, CancellationToken cancellationToken)
    {
        var overview = await _financialRecordRepository.GetIncomeExpenseOverviewAsync(
            request.StartDate,
            request.EndDate,
            request.ResponsibleEmployeeId,
            request.ApprovedById);

        return _mapper.Map<IncomeExpenseOverviewDto>(overview);
    }

    public async Task<FinancialRecordResult> Handle(GetFinancialRecordsByTypeQuery request, CancellationToken cancellationToken)
    {
        var records = await _financialRecordRepository.GetByTypeAsync(
            request.TransactionType,
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.PaymentMethod,
            request.ResponsibleEmployeeId,
            request.ApprovedById,
            request.MinAmount,
            request.MaxAmount,
            request.SortBy,
            request.Descending,
            request.Page,
            request.PageSize);

        var totalCount = await _financialRecordRepository.CountByTypeAsync(
            request.TransactionType,
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.PaymentMethod,
            request.ResponsibleEmployeeId,
            request.ApprovedById,
            request.MinAmount,
            request.MaxAmount);

        var summaryDtos = _mapper.Map<List<FinancialRecordSummaryDto>>(records);

        return new FinancialRecordResult
        {
            FinancialRecords = summaryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
