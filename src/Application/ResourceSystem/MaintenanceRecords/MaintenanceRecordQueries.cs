using MediatR;  
using DbApp.Domain.Enums.ResourceSystem;  
  
namespace DbApp.Application.ResourceSystem.MaintenanceRecords;  
  
// 查询类  
public record GetMaintenanceRecordByIdQuery(int MaintenanceId) : IRequest<MaintenanceRecordSummaryDto?>;  
public record SearchMaintenanceRecordsQuery(string? SearchTerm, int Page = 1, int PageSize = 10) : IRequest<MaintenanceRecordResult>;  
public record SearchMaintenanceRecordsByRideQuery(int RideId, int Page = 1, int PageSize = 10) : IRequest<MaintenanceRecordResult>;  
public record SearchMaintenanceRecordsByStatusQuery(bool IsCompleted, bool? IsAccepted = null, int Page = 1, int PageSize = 10) : IRequest<MaintenanceRecordResult>;  
public record GetMaintenanceRecordStatsQuery(DateTime? StartDate = null, DateTime? EndDate = null) : IRequest<MaintenanceRecordStatsDto>;  
  
// 命令类 - 新增CRUD操作  
public record CreateMaintenanceRecordCommand(  
    int RideId,  
    int TeamId,  
    int? ManagerId,  
    MaintenanceType MaintenanceType,  
    DateTime StartTime,  
    DateTime? EndTime,  
    decimal Cost,  
    string? PartsReplaced,  
    string? MaintenanceDetails,  
    bool IsCompleted,  
    bool? IsAccepted,  
    DateTime? AcceptanceDate,  
    string? AcceptanceComments  
) : IRequest<int>;  
  
public record UpdateMaintenanceRecordCommand(  
    int MaintenanceId,  
    int RideId,  
    int TeamId,  
    int? ManagerId,  
    MaintenanceType MaintenanceType,  
    DateTime StartTime,  
    DateTime? EndTime,  
    decimal Cost,  
    string? PartsReplaced,  
    string? MaintenanceDetails,  
    bool IsCompleted,  
    bool? IsAccepted,  
    DateTime? AcceptanceDate,  
    string? AcceptanceComments  
) : IRequest;  
  
public record DeleteMaintenanceRecordCommand(int MaintenanceId) : IRequest<bool>;