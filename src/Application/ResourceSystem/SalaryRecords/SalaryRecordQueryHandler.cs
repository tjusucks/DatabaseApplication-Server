using AutoMapper;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.SalaryRecords;

public class SalaryRecordQueryHandler(
    ISalaryRecordRepository salaryRecordRepository,
    IMapper mapper) :
    IRequestHandler<SearchSalaryRecordQuery, SalaryRecordResult>,
    IRequestHandler<GetSalaryRecordByIdQuery, SalaryRecordDetailDto?>,
    IRequestHandler<GetSalaryStatsQuery, SalaryStatsDto>,
    IRequestHandler<GetGroupedSalaryStatsQuery, List<GroupedSalaryStatsDto>>,
    IRequestHandler<GetEmployeeSalaryRecordsQuery, SalaryRecordResult>,
    IRequestHandler<GetEmployeeSalarySummaryQuery, EmployeeSalarySummaryDto?>,
    IRequestHandler<GetMonthlySalaryReportQuery, List<MonthlySalaryReportDto>>,
    IRequestHandler<GetPayrollQuery, List<SalaryRecordSummaryDto>>
{
    private readonly ISalaryRecordRepository _salaryRecordRepository = salaryRecordRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<SalaryRecordResult> Handle(SearchSalaryRecordQuery request, CancellationToken cancellationToken)
    {
        var records = await _salaryRecordRepository.SearchAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.EmployeeId,
            request.Position,
            request.DepartmentName,
            request.StaffType,
            request.EmploymentStatus,
            request.MinSalary,
            request.MaxSalary,
            request.SortBy,
            request.Descending,
            request.Page,
            request.PageSize);

        var totalCount = await _salaryRecordRepository.CountAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.EmployeeId,
            request.Position,
            request.DepartmentName,
            request.StaffType,
            request.EmploymentStatus,
            request.MinSalary,
            request.MaxSalary);

        var summaryDtos = _mapper.Map<List<SalaryRecordSummaryDto>>(records);

        return new SalaryRecordResult
        {
            SalaryRecords = summaryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<SalaryRecordDetailDto?> Handle(GetSalaryRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var record = await _salaryRecordRepository.GetByIdAsync(request.SalaryRecordId);
        return record == null ? null : _mapper.Map<SalaryRecordDetailDto>(record);
    }

    public async Task<SalaryStatsDto> Handle(GetSalaryStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await _salaryRecordRepository.GetStatsAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.EmployeeId,
            request.Position,
            request.DepartmentName,
            request.StaffType,
            request.EmploymentStatus,
            request.MinSalary,
            request.MaxSalary);

        return _mapper.Map<SalaryStatsDto>(stats);
    }

    public async Task<List<GroupedSalaryStatsDto>> Handle(GetGroupedSalaryStatsQuery request, CancellationToken cancellationToken)
    {
        var groupedStats = await _salaryRecordRepository.GetGroupedStatsAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.EmployeeId,
            request.Position,
            request.DepartmentName,
            request.StaffType,
            request.EmploymentStatus,
            request.MinSalary,
            request.MaxSalary,
            request.GroupBy,
            request.SortBy,
            request.Descending);

        return _mapper.Map<List<GroupedSalaryStatsDto>>(groupedStats);
    }

    public async Task<SalaryRecordResult> Handle(GetEmployeeSalaryRecordsQuery request, CancellationToken cancellationToken)
    {
        var records = await _salaryRecordRepository.GetByEmployeeAsync(
            request.EmployeeId,
            request.StartDate,
            request.EndDate,
            request.SortBy,
            request.Descending,
            request.Page,
            request.PageSize);

        var totalCount = await _salaryRecordRepository.CountByEmployeeAsync(
            request.EmployeeId,
            request.StartDate,
            request.EndDate);

        var summaryDtos = _mapper.Map<List<SalaryRecordSummaryDto>>(records);

        return new SalaryRecordResult
        {
            SalaryRecords = summaryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<EmployeeSalarySummaryDto?> Handle(GetEmployeeSalarySummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = await _salaryRecordRepository.GetEmployeeSalarySummaryAsync(
            request.EmployeeId,
            request.StartDate,
            request.EndDate);

        return summary == null ? null : _mapper.Map<EmployeeSalarySummaryDto>(summary);
    }

    public async Task<List<MonthlySalaryReportDto>> Handle(GetMonthlySalaryReportQuery request, CancellationToken cancellationToken)
    {
        var reports = await _salaryRecordRepository.GetMonthlySalaryReportAsync(
            request.Year,
            request.Month,
            request.DepartmentName,
            request.StaffType,
            request.SortBy,
            request.Descending);

        return _mapper.Map<List<MonthlySalaryReportDto>>(reports);
    }

    public async Task<List<SalaryRecordSummaryDto>> Handle(GetPayrollQuery request, CancellationToken cancellationToken)
    {
        var records = await _salaryRecordRepository.GetPayrollAsync(
            request.PayDate,
            request.DepartmentName,
            request.StaffType,
            request.EmploymentStatus);

        return _mapper.Map<List<SalaryRecordSummaryDto>>(records);
    }
}
