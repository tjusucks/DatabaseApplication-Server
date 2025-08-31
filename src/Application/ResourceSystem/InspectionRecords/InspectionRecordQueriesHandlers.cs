using AutoMapper;  
using DbApp.Domain.Interfaces.ResourceSystem;  
using MediatR;  
  
namespace DbApp.Application.ResourceSystem.InspectionRecords;  
  
public class InspectionRecordQueryHandler(  
    IInspectionRecordRepository inspectionRecordRepository,  
    IMapper mapper) :  
    IRequestHandler<GetInspectionRecordByIdQuery, InspectionRecordSummaryDto?>,  
    IRequestHandler<SearchInspectionRecordsQuery, InspectionRecordResult>,  
    IRequestHandler<SearchInspectionRecordsByRideQuery, InspectionRecordResult>,  
    IRequestHandler<GetInspectionRecordStatsQuery, InspectionRecordStatsDto>,  
    IRequestHandler<CreateInspectionRecordCommand, int>,  
    IRequestHandler<UpdateInspectionRecordCommand>,  
    IRequestHandler<DeleteInspectionRecordCommand, bool>  
{  
    private readonly IInspectionRecordRepository _inspectionRecordRepository = inspectionRecordRepository;  
    private readonly IMapper _mapper = mapper;  
  
    // 查询处理方法  
    public async Task<InspectionRecordSummaryDto?> Handle(GetInspectionRecordByIdQuery request, CancellationToken cancellationToken)  
    {  
        var record = await _inspectionRecordRepository.GetByIdAsync(request.InspectionId);  
        return record == null ? null : _mapper.Map<InspectionRecordSummaryDto>(record);  
    }  
  
    public async Task<InspectionRecordResult> Handle(SearchInspectionRecordsQuery request, CancellationToken cancellationToken)  
    {  
        var records = await _inspectionRecordRepository.SearchAsync(request.SearchTerm, request.Page, request.PageSize);  
        var totalCount = await _inspectionRecordRepository.CountAsync(request.SearchTerm);  
        var recordDtos = _mapper.Map<List<InspectionRecordSummaryDto>>(records);  
  
        return new InspectionRecordResult  
        {  
            InspectionRecords = recordDtos,  
            TotalCount = totalCount,  
            Page = request.Page,  
            PageSize = request.PageSize  
        };  
    }  
  
    public async Task<InspectionRecordResult> Handle(SearchInspectionRecordsByRideQuery request, CancellationToken cancellationToken)  
    {  
        var records = await _inspectionRecordRepository.SearchByRideAsync(request.RideId, request.Page, request.PageSize);  
        var totalCount = await _inspectionRecordRepository.CountByRideAsync(request.RideId);  
        var recordDtos = _mapper.Map<List<InspectionRecordSummaryDto>>(records);  
  
        return new InspectionRecordResult  
        {  
            InspectionRecords = recordDtos,  
            TotalCount = totalCount,  
            Page = request.Page,  
            PageSize = request.PageSize  
        };  
    }  
  
    public async Task<InspectionRecordStatsDto> Handle(GetInspectionRecordStatsQuery request, CancellationToken cancellationToken)  
    {  
        var stats = await _inspectionRecordRepository.GetStatsAsync(request.StartDate, request.EndDate);  
        return _mapper.Map<InspectionRecordStatsDto>(stats);  
    }  
  
    // CRUD命令处理方法  
    public async Task<int> Handle(CreateInspectionRecordCommand request, CancellationToken cancellationToken)  
    {  
        var record = new Domain.Entities.ResourceSystem.InspectionRecord  
        {  
            RideId = request.RideId,  
            TeamId = request.TeamId,  
            CheckDate = request.CheckDate,  
            CheckType = request.CheckType,  
            IsPassed = request.IsPassed,  
            IssuesFound = request.IssuesFound,  
            Recommendations = request.Recommendations,  
            CreatedAt = DateTime.UtcNow,  
            UpdatedAt = DateTime.UtcNow  
        };  
  
        await _inspectionRecordRepository.AddAsync(record);  
        return record.InspectionId;  
    }  
  
    public async Task Handle(UpdateInspectionRecordCommand request, CancellationToken cancellationToken)  
    {  
        var record = await _inspectionRecordRepository.GetByIdAsync(request.InspectionId);  
  
        if (record == null)  
        {  
            throw new InvalidOperationException("Inspection record not found");  
        }  
  
        record.RideId = request.RideId;  
        record.TeamId = request.TeamId;  
        record.CheckDate = request.CheckDate;  
        record.CheckType = request.CheckType;  
        record.IsPassed = request.IsPassed;  
        record.IssuesFound = request.IssuesFound;  
        record.Recommendations = request.Recommendations;  
        record.UpdatedAt = DateTime.UtcNow;  
  
        await _inspectionRecordRepository.UpdateAsync(record);  
    }  
  
    public async Task<bool> Handle(DeleteInspectionRecordCommand request, CancellationToken cancellationToken)  
    {  
        var record = await _inspectionRecordRepository.GetByIdAsync(request.InspectionId);  
  
        if (record == null)  
        {  
            return false;  
        }  
  
        await _inspectionRecordRepository.DeleteAsync(record);  
        return true;  
    }  
}