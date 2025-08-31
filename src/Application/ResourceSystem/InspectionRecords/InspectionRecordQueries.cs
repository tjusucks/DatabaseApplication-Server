using MediatR;  
using DbApp.Domain.Enums.ResourceSystem;  
  
namespace DbApp.Application.ResourceSystem.InspectionRecords;  
  
// 查询类  
public record GetInspectionRecordByIdQuery(int InspectionId) : IRequest<InspectionRecordSummaryDto?>;  
public record SearchInspectionRecordsQuery(string? SearchTerm, int Page = 1, int PageSize = 10) : IRequest<InspectionRecordResult>;  
public record SearchInspectionRecordsByRideQuery(int RideId, int Page = 1, int PageSize = 10) : IRequest<InspectionRecordResult>;  
public record GetInspectionRecordStatsQuery(DateTime? StartDate = null, DateTime? EndDate = null) : IRequest<InspectionRecordStatsDto>;  
  
// 命令类 - 新增CRUD操作  
public record CreateInspectionRecordCommand(  
    int RideId,  
    int TeamId,  
    DateTime CheckDate,  
    CheckType CheckType,  
    bool IsPassed,  
    string? IssuesFound,  
    string? Recommendations  
) : IRequest<int>;  
  
public record UpdateInspectionRecordCommand(  
    int InspectionId,  
    int RideId,  
    int TeamId,  
    DateTime CheckDate,  
    CheckType CheckType,  
    bool IsPassed,  
    string? IssuesFound,  
    string? Recommendations  
) : IRequest;  
  
public record DeleteInspectionRecordCommand(int InspectionId) : IRequest<bool>;