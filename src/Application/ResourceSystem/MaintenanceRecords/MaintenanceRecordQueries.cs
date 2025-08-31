using MediatR;  
using DbApp.Domain.Enums.ResourceSystem;  
  
namespace DbApp.Application.ResourceSystem.MaintenanceRecords;  
  
/// <summary>  
/// Query to get maintenance record by ID.  
/// </summary>  
public record GetMaintenanceRecordByIdQuery(int MaintenanceId) : IRequest<MaintenanceRecordSummaryDto?>;  
  
/// <summary>  
/// Query to search maintenance records with filtering options.  
/// </summary>  
public record SearchMaintenanceRecordsQuery(  
    string? SearchTerm,   
    int Page = 1,   
    int PageSize = 10  
) : IRequest<MaintenanceRecordResult>;  
  
/// <summary>  
/// Query to search maintenance records by ride.  
/// </summary>  
public record SearchMaintenanceRecordsByRideQuery(  
    int RideId,   
    int Page = 1,   
    int PageSize = 10  
) : IRequest<MaintenanceRecordResult>;  
  
/// <summary>  
/// Query to search maintenance records by status.  
/// </summary>  
public record SearchMaintenanceRecordsByStatusQuery(  
    bool IsCompleted,  
    bool? IsAccepted = null,  
    int Page = 1,   
    int PageSize = 10  
) : IRequest<MaintenanceRecordResult>;  
  
/// <summary>  
/// Query to get maintenance record statistics.  
/// </summary>  
public record GetMaintenanceRecordStatsQuery(  
    DateTime? StartDate = null,   
    DateTime? EndDate = null  
) : IRequest<MaintenanceRecordStatsDto>;